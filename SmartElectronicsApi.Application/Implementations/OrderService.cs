using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Color;
using SmartElectronicsApi.Application.Dtos.Order;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using Stripe;
using Stripe.Checkout;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            _emailService = emailService;
        }

        public async Task<string> CreateStripeCheckoutSessionAsync(int addressId)
        {
            using (var transaction = await _unitOfWork.OrderRepository.BeginTransactionAsync())
            {
                try
                {
                    var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                        throw new CustomException(400, "User ID cannot be null");

                    var user = await _userManager.FindByIdAsync(userId);

                    var basketProducts = await _unitOfWork.BasketProductRepository
                        .GetAll(bp => bp.Basket.AppUserId == userId, includes: new Func<IQueryable<BasketProduct>, IQueryable<BasketProduct>>[]
                        {
                            query => query.Include(c => c.Product).ThenInclude(s => s.Variations).Include(s=>s.Product).ThenInclude(s=>s.productImages)
                        });

                    if (!basketProducts.Any())
                    {
                        throw new Exception("No products in the basket to create an order.");
                    }

                    var address = await _unitOfWork.addressRepository.GetEntity(s => s.Id == addressId);
                    if (address == null || address.AppUserId != userId)
                    {
                        throw new Exception("Invalid address selected for shipping.");
                    }

                    var orderItems = basketProducts.Select(bp => new OrderItem
                    {
                        ProductId = (int)bp.ProductId,
                        Product = bp.Product,
                        ProductVariationId = bp.ProductVariationId,
                        ProductVariation = bp.ProductVariation,
                        Quantity = bp.Quantity,
                        UnitPrice = (decimal)((bp.ProductVariation != null && bp.ProductVariation.DiscountedPrice.HasValue && bp.ProductVariation.DiscountedPrice.Value > 0)
                            ? bp.ProductVariation.DiscountedPrice
                            : (bp.Product.DiscountedPrice > 0 ? bp.Product.DiscountedPrice : bp.Product.Price))
                    }).ToList();

                    var totalAmount = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
                    var tax = totalAmount * 0.1m;
                    var shippingCost = 15.00m;

                    // Apply loyalty points discount
                    if (user.loyalPoints >= 100 && user.loyalPoints <= 500)
                    {
                        totalAmount -= (totalAmount * 5) / 100;
                    }
                    else if (user.loyalPoints > 500 && user.loyalPoints < 800)
                    {
                        totalAmount -= (totalAmount * 8) / 100;
                    }
                    else if (user.loyalPoints > 800 && user.loyalPoints < 1000)
                    {
                        totalAmount -= (totalAmount * 12) / 100;
                    }

                    var order = new Order
                    {
                        OrderDate = DateTime.Now,
                        TotalAmount = totalAmount + tax + shippingCost,
                        Tax = tax,
                        ShippingCost = shippingCost,
                        ShippingAddress = $"{address.Street}, {address.City}, {address.State}, {address.ZipCode}, {address.Country}",
                        Status = OrderStatus.Pending,
                        AppUserId = userId,
                        Items = orderItems
                    };

                    await _unitOfWork.OrderRepository.Create(order);
                    _unitOfWork.Commit();
                    foreach(var item in orderItems)
                    {
                        await _unitOfWork.OrderItemRepository.Create(item);
                        _unitOfWork.Commit();
                    }
                   
                    // Delete basket products
                    foreach (var basketProduct in basketProducts)
                    {
                        await _unitOfWork.BasketProductRepository.Delete(basketProduct);
                    }
                    _unitOfWork.Commit();

                    // Create Stripe checkout session
                    var lineItems = orderItems.Select(item => new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.UnitPrice * 100),
                            Currency = "azn",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                                Images = new List<string> { item.Product.productImages.FirstOrDefault(s=>s.IsMain==true).Name }
                            },
                        },
                        Quantity = item.Quantity,
                    }).ToList();

                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },
                        LineItems = lineItems,
                        Mode = "payment",
                        SuccessUrl = $"http://localhost:5246/api/Order/verify-payment?sessionId={{CHECKOUT_SESSION_ID}}",
                        CancelUrl = $"http://localhost:5246/api/Order/verify-payment?sessionId={{CHECKOUT_SESSION_ID}}",
                        Metadata = new Dictionary<string, string>
    {
        { "orderId", order.Id.ToString() }
    }
                    };

                    var service = new SessionService();
                    Session session = await service.CreateAsync(options);
                
                    // Loyalty points logic
                    if (user.loyalPoints < 1000)
                    {
                        if (totalAmount >= 10000)
                        {
                            user.loyalPoints += (int)totalAmount / 1500;
                        }
                        else if (totalAmount < 10000 && totalAmount >= 1000)
                        {
                            user.loyalPoints += (int)totalAmount / 3500;
                        }

                        // Upgrade loyalty tier when points reach 1000
                        if (user.loyalPoints >= 1000)
                        {
                            user.loyalPoints = 100;
                            user.loyaltyTier += 1;
                        }
                    }

                    await _userManager.UpdateAsync(user);

                    await transaction.CommitAsync();

                    // Send confirmation email
                    string templatePath = "wwwroot/Template/Invoice.html";
                    string body;
                    using (StreamReader reader = new StreamReader(templatePath))
                    {
                        body = await reader.ReadToEndAsync();
                    }
                    body = body.Replace("{{CustomerName}}", user.UserName)
                             .Replace("{{InvoiceDate}}", order.OrderDate.ToString("dd MMM yyyy"))
                             .Replace("{{SubTotal}}", totalAmount.ToString("C"))
                             .Replace("{{Tax}}", tax.ToString("C"))
                             .Replace("{{Total}}", order.TotalAmount.ToString("C"));

                    // Generate order items for email
                    StringBuilder orderItemsBuilder = new StringBuilder();
                    foreach (var item in orderItems)
                    {
                        orderItemsBuilder.Append($@"
                        <tr>
                            <td>{item.Product.Name}</td>
                            <td>{item.Quantity}</td>
                            <td>{item.UnitPrice:C}</td>
                            <td>{(item.Quantity * item.UnitPrice):C}</td>
                        </tr>");
                    }
                    body = body.Replace("{{OrderItems}}", orderItemsBuilder.ToString());

                    _emailService.SendEmail(
                        from: "nihadmi@code.edu.az",
                        to: user.Email,
                        subject: $"Invoice for Order #{order.Id}",
                        body: body,
                    smtpHost: "smtp.gmail.com",
                    smtpPort: 587,
                    enableSsl: true,
                    smtpUser: "nihadmi@code.edu.az\r\n",
                    smtpPass: "jytx krmh ngqj vdnc\r\n"
                    );

                    return session.Id;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Order creation failed: {ex.Message}");
                }
            }
        }
        public async Task<string> VerifyPayment(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            if (session.PaymentStatus == "paid")
            {
                var orderId = session.Metadata["orderId"];
                var order = await _unitOfWork.OrderRepository.GetEntity(o => o.Id == int.Parse(orderId));

                if (order != null)
                {
                    order.Status = OrderStatus.Completed;
                  await  _unitOfWork.OrderRepository.Update(order);
                     _unitOfWork.Commit();  
                }
                return "Payment successful"; 
            }
            else
            {
                return "Payment failed or incomplete";
            }
        }
        public async Task<PaginatedResponse<OrderListItemDto>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var totalCount = (await _unitOfWork.OrderRepository.GetAll()).Count();
            var orders = await _unitOfWork.OrderRepository.GetAll(s => s.IsDeleted == false,
                                                                  (pageNumber - 1) * pageSize,
                                                                  pageSize, includes: new Func<IQueryable<Order>, IQueryable<Order>>[]
                {
        query => query.Include(c => c.Items).ThenInclude(s=>s.Product)
                });

            var MappedOrders=_mapper.Map<List<OrderListItemDto>>(orders);
            return new PaginatedResponse<OrderListItemDto>
            {
                Data = MappedOrders,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<PaginatedResponse<OrderListItemDto>> GetAllForUser(int pageNumber = 1, int pageSize = 10)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new CustomException(400, "User ID cannot be null");

            var user = await _userManager.FindByIdAsync(userId);
            var totalCount = (await _unitOfWork.OrderRepository.GetAll()).Count();
            var orders = await _unitOfWork.OrderRepository.GetAll(s => s.IsDeleted == false&&s.AppUserId==user.Id,
                                                                  (pageNumber - 1) * pageSize,
                                                                  pageSize, includes: new Func<IQueryable<Order>, IQueryable<Order>>[]
                {
        query => query.Include(c => c.Items).ThenInclude(s=>s.Product)
                });

            var MappedOrders = _mapper.Map<List<OrderListItemDto>>(orders);
            return new PaginatedResponse<OrderListItemDto>
            {
                Data = MappedOrders,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
