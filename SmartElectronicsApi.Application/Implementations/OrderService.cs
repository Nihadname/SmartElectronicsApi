using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos.Order;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
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
        private  readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<OrderListItemDto> CreateOrderAsync(int addressId)
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
                    OrderDate = DateTime.UtcNow,
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
                var MappedOrder=_mapper.Map<OrderListItemDto>(order);
                return MappedOrder;
      
            
        }


    }
}
