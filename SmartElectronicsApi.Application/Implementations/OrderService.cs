using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartElectronicsApi.Application.Dtos.Order;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using Stripe;
using Stripe.Issuing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class OrderService:IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _configuration = configuration;

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"]; 
            _emailService = emailService;
        }

        public async Task<OrderListItemDto> CreateOrderAsync(int addressId,string token)
        {
              var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    throw new CustomException(400, "User ID cannot be null");

                var user = await _userManager.FindByIdAsync(userId);
                // Get the user's basket products
                var basketProducts = await _unitOfWork.BasketProductRepository
                    .GetAll(bp => bp.Basket.AppUserId == userId, includes: new Func<IQueryable<BasketProduct>, IQueryable<BasketProduct>>[]
            {
        query => query.Include(c => c.Product).ThenInclude(s=>s.Variations)
                      
            });

                if (!basketProducts.Any())
                {
                    throw new Exception("No products in the basket to create an order.");
                }
                var address = await _unitOfWork.addressRepository
           .GetEntity(s => s.Id == addressId);

                if (address == null || address.AppUserId != userId)
                {
                    throw new Exception("Invalid address selected for shipping.");
                }

                // Map basket products to order items
                var orderItems = basketProducts.Select(bp => new OrderItem
                {
                    ProductId = (int)bp.ProductId,
                    Product = bp.Product,
                    ProductVariationId = bp.ProductVariationId,
                    ProductVariation = bp.ProductVariation,
                    Quantity = bp.Quantity,
                    UnitPrice = (decimal)((bp.ProductVariation != null && bp.ProductVariation.DiscountedPrice.HasValue && bp.ProductVariation.DiscountedPrice.Value > 0)
                ? bp.ProductVariation.DiscountedPrice // Use ProductVariation's discounted price
                : (bp.Product.DiscountedPrice > 0 ? bp.Product.DiscountedPrice : bp.Product.Price)) // Fallback to Product's discounted price or price
                }).ToList();

                var totalAmount = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
                var tax = totalAmount * 0.1m;
                var shippingCost = 15.00m;

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

                foreach (var basketProduct in basketProducts)
                {
                    await _unitOfWork.BasketProductRepository.Delete(basketProduct);
                }
                _unitOfWork.Commit();
            var paymentService = new ChargeService();
            var chargeOptions = new ChargeCreateOptions
            {
                Amount = (long)(order.TotalAmount * 100), // Amount in cents
                Currency = "azn", // Currency
                Source = token,// Pass the actual payment source
                Description = $"Order #{order.Id} payment",
            };

            try
            {
                Charge charge = await paymentService.CreateAsync(chargeOptions);

                // Check if the charge was successful
                if (charge.Status != "succeeded")
                {
                    throw new Exception("Payment processing failed.");
                }
                order.Status = OrderStatus.Completed;
                _unitOfWork.Commit();
                // Send confirmation email using your EmailService
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

                // Dynamically generate the order items
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

            }
            catch (StripeException ex)
            {
                order.Status = OrderStatus.Failed;
                _unitOfWork.Commit();
                // Handle Stripe-specific exceptions
                throw new Exception($"Payment processing failed: {ex.Message}");
            }
            var MappedOrder =_mapper.Map<OrderListItemDto>(order);
                return MappedOrder;
      
            
        }


    }
}
