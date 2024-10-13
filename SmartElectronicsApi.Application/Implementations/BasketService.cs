using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos.Basket;
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
    public class BasketService:IBasketService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper ;
        public BasketService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<UserBasketDto> GetUserBasket()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new CustomException(400, "User ID cannot be null");

            var user = await _userManager.FindByIdAsync(userId);
            var Basket = await _unitOfWork.BasketRepository.GetEntity(s => s.AppUserId == userId, includes: new Func<IQueryable<Basket>, IQueryable<Basket>>[] {
query => query
    .Include(s => s.BasketProducts)
        .ThenInclude(bp => bp.Product)
        .ThenInclude(p => p.productImages) 
    .Include(s => s.BasketProducts)
        .ThenInclude(bp => bp.ProductVariation) 
        .ThenInclude(pv => pv.productImages)
         .Include(s => s.BasketProducts)
        .ThenInclude(bp => bp.Product)
                    .ThenInclude(p => p.Category) 

        });
            if (Basket == null) throw new CustomException(404, " Not found.");
            var userBasketDto = _mapper.Map<UserBasketDto>(Basket);
            return userBasketDto;
        }
        public async Task<int> Add(int? productId, int? variationId = null)
        {
            
            if (productId == null)
                throw new CustomException(400, "ProductId cannot be null");

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new CustomException(400, "User ID cannot be null");

            var user = await _userManager.FindByIdAsync(userId);
            var existedProduct = await _unitOfWork.productRepository.GetEntity(s => s.Id == productId);
            if (existedProduct is null)
                throw new CustomException(404, "Product not found");

            ProductVariation existedVariation = null;

            // Check if a variation was provided
            if (variationId.HasValue)
            {
                existedVariation = await _unitOfWork.productVariationRepository.GetEntity(s => s.Id == variationId && s.ProductId == productId);
                if (existedVariation == null)
                    throw new CustomException(404, "Variation not found");
            }

            var existedBasket = await _unitOfWork.BasketRepository.GetEntity(s => s.AppUserId == user.Id, includes: new Func<IQueryable<Basket>, IQueryable<Basket>>[]
            {
        query => query.Include(c => c.BasketProducts)
                      .ThenInclude(bp => bp.Product)
                      .ThenInclude(p => p.Variations)
            });

            var basketProduct = await _unitOfWork.BasketProductRepository.GetEntity(s =>
                s.ProductId == productId &&
                (variationId.HasValue ? s.ProductVariationId == variationId : s.ProductVariationId == null) &&
                s.Basket.AppUserId == user.Id
            );

            if (basketProduct != null)
            {
                basketProduct.Quantity++;
                _unitOfWork.Commit();
                var updatedBasket = await GetUserBasket();
                var basketCount = updatedBasket.BasketProducts.Sum(item => item.Quantity);

                return basketCount;
            }

            if (existedBasket is not null)
            {
                existedBasket.BasketProducts.Add(new BasketProduct()
                {
                    Quantity = 1,
                    BasketId = existedBasket.Id,
                    ProductId = existedProduct.Id,
                    ProductVariationId = existedVariation?.Id  
                });
                _unitOfWork.Commit();
                var updatedBasket1 = await GetUserBasket();
                var basketCount1 = updatedBasket1.BasketProducts.Sum(item => item.Quantity);

                return basketCount1;
            }

            Basket newBasket = new()
            {
                AppUserId = user.Id,
            };
            await _unitOfWork.BasketRepository.Create(newBasket);
            _unitOfWork.Commit();

            BasketProduct newBasketProduct = new()
            {
                Quantity = 1,
                BasketId = newBasket.Id,
                ProductId = existedProduct.Id,
                ProductVariationId = existedVariation?.Id  // Set the variation only if it exists
            };
            await _unitOfWork.BasketProductRepository.Create(newBasketProduct);
            _unitOfWork.Commit();

            var updatedBasket2 = await GetUserBasket();
            var basketCount2 = updatedBasket2.BasketProducts.Sum(item => item.Quantity);

            return basketCount2;
        }
        public async Task<int> ChangeQuantity(int? productId, int? variationId = null, int quantityChange = 1)
        {
            if (productId == null)
                throw new CustomException(400, "ProductId cannot be null");

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new CustomException(400, "User ID cannot be null");

            var user = await _userManager.FindByIdAsync(userId);
            var existedProduct = await _unitOfWork.productRepository.GetEntity(s => s.Id == productId);
            if (existedProduct == null)
                throw new CustomException(404, "Product not found");

            ProductVariation existedVariation = null;

            // Check if a variation was provided
            if (variationId.HasValue)
            {
                existedVariation = await _unitOfWork.productVariationRepository.GetEntity(s => s.Id == variationId && s.ProductId == productId);
                if (existedVariation == null)
                    throw new CustomException(404, "Variation not found");
            }

            var basketProduct = await _unitOfWork.BasketProductRepository.GetEntity(s =>
                s.ProductId == productId &&
                (variationId.HasValue ? s.ProductVariationId == variationId : s.ProductVariationId == null) &&
                s.Basket.AppUserId == user.Id
            );

            if (basketProduct == null)
                throw new CustomException(404, "Product not found in the basket");

            // Modify the quantity based on the provided change value
            basketProduct.Quantity += quantityChange;

            // Ensure the quantity doesn't go below 1
            if (basketProduct.Quantity <= 0)
            {
                // Remove the item from the basket if the quantity drops to 0 or below
                _unitOfWork.BasketProductRepository.Delete(basketProduct);
            }
            else
            {
                _unitOfWork.BasketProductRepository.Update(basketProduct);
            }

            _unitOfWork.Commit();

            var updatedBasket = await GetUserBasket();
            var basketCount = updatedBasket.BasketProducts.Sum(item => item.Quantity);

            return basketCount;
        }

    }
}
