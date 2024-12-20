using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QRCoder;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Order;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using Stripe;
using Stripe.Checkout;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Claims;
using System.Text;
using Color = System.Drawing.Color;

namespace SmartElectronicsApi.Application.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<Core.Entities.AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ICodeGeneratorService _codegenService;
        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            IEmailService emailService,
            ICodeGeneratorService codegenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            _emailService = emailService;
            _codegenService = codegenService;
        }

        public async Task<string> CreateStripeCheckoutSessionAsync()
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
                        throw new CustomException(400,"No products in the basket to create an order.");
                    }

                    var address = await _unitOfWork.addressRepository.GetEntity(s=>s.AppUserId==userId);
                    if (address == null || address.AppUserId != userId)
                    {
                        throw new CustomException(400,"Invalid address selected for shipping., pls go to profile page to create address");
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
                   
                    foreach (var basketProduct in basketProducts)
                    {
                        await _unitOfWork.BasketProductRepository.Delete(basketProduct);
                    }
                    _unitOfWork.Commit();

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
                    smtpPass: "eise hosy kfne qhnm"
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
        public async Task<PaginatedResponse<OrderAdminListItemDto>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var totalCount = (await _unitOfWork.OrderRepository.GetAll()).Count();
            var orders = await _unitOfWork.OrderRepository.GetAll(s => s.IsDeleted == false,
                                                                  (pageNumber - 1) * pageSize,
                                                                  pageSize, includes: new Func<IQueryable<Order>, IQueryable<Order>>[]
                {
        query => query.Include(c => c.AppUser).ThenInclude(s=>s.orders).ThenInclude(s=>s.Items)
                });

            var MappedOrders=_mapper.Map<List<OrderAdminListItemDto>>(orders);
            return new PaginatedResponse<OrderAdminListItemDto>
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
        public async Task<string> ShippingOrder(int? Id)
        {
            if (Id is null) throw new CustomException(400, "Id", "id can't be null");

            var existedOrder = await _unitOfWork.OrderRepository.GetEntity(
                s => s.Id == Id && s.IsDeleted == false,
                includes: new Func<IQueryable<Order>, IQueryable<Order>>[]
                {
            query => query.Include(p => p.AppUser)
                }
            );

            if (existedOrder == null)
            {
                throw new CustomException(400, "existedOrder", "existedOrder can't be null");
            }

            if (existedOrder.Status == OrderStatus.Completed)
            {
                existedOrder.Status = OrderStatus.Shipped;
                existedOrder.ShippedToken = _codegenService.GenerateCode();
                await _unitOfWork.OrderRepository.Update(existedOrder);
                _unitOfWork.Commit();

                var guid = Guid.NewGuid().ToString();
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                    ("Shipped Token: " + existedOrder.ShippedToken + " OrderId: " + existedOrder.Id) + "\r\n" + guid,
                    QRCodeGenerator.ECCLevel.Q
                );
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap logoImage = new Bitmap(@"wwwroot/img/logo size-02.png");
                using (Bitmap qrCodeAsBitmap = qrCode.GetGraphic(20, Color.Black, Color.WhiteSmoke, logoImage))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrCodeAsBitmap.Save(ms, ImageFormat.Png);

                        string emailBody = $@"
                <html>
                    <body>
                        <h1>Order Shipped</h1>
                        <p>Your order has been shipped. Below is the QR code for your reference:</p>
                        <img src='cid:QRCodeImage' alt='QR Code' />
                        <p>Order ID: {existedOrder.Id}</p>
                        <p>Shipped Token: {existedOrder.ShippedToken}</p>
                    </body>
                </html>";

                        var attachment = new System.Net.Mail.Attachment(new MemoryStream(ms.ToArray()), "QRCode.png");
                        attachment.ContentId = "QRCodeImage";
                        attachment.ContentDisposition.Inline = true;
                        attachment.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;

                        var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
                        {
                            Port = 587,
                            Credentials = new System.Net.NetworkCredential("nihadmi@code.edu.az", "eise hosy kfne qhnm"),
                            EnableSsl = true,
                        };

                        var mailMessage = new System.Net.Mail.MailMessage
                        {
                            From = new System.Net.Mail.MailAddress("nihadmi@code.edu.az"),
                            Subject = "Order Already Shipped",
                            Body = emailBody,
                            IsBodyHtml = true
                        };

                        mailMessage.To.Add(existedOrder.AppUser.Email);
                        mailMessage.Attachments.Add(attachment);

                        await smtpClient.SendMailAsync(mailMessage);

                        return "Email sent successfully with QR code attached and embedded.";
                    }
                }
            }

            return "empty";
        }
        public async Task<string> VerifyOrderAsDelivered(OrderVerifyDto orderVerifyDto)
        {
            var existedUser = await _userManager.FindByNameAsync(orderVerifyDto.UserName);
            if (existedUser == null) throw new CustomException(400, "User", "User can not be null");
          var existedOrder=await _unitOfWork.OrderRepository.GetEntity(s=>s.Id== orderVerifyDto.Id&&s.IsDeleted==false);
            if(existedOrder == null) throw new CustomException(400, "Order", "Order can not be null");
            var isOrderExistedInUser=await _unitOfWork.OrderRepository.isExists(s=>s.Id == existedOrder.Id&&s.AppUserId==existedUser.Id&&s.IsDeleted==false);
            if(!isOrderExistedInUser) throw new CustomException(400, "Order", "Order is not on the list of given user");
            if (existedOrder.Status != OrderStatus.Shipped) throw new CustomException(400, "this order not shipped yet");
            if(existedOrder.Status == OrderStatus.Delivered) throw new CustomException(400, "this order is already delivered ");
            if (!existedOrder.ShippedToken.Equals(orderVerifyDto.ShippedToken)) throw new CustomException(400, "given ahipped token doesnt match ");
            existedOrder.Status=OrderStatus.Delivered;
            await _unitOfWork.OrderRepository.Update(existedOrder);
            _unitOfWork.Commit();
            return "Order delivery confirmed successfully.";
        }
    }
}
