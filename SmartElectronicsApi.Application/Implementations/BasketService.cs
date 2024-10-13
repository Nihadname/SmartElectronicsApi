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
        .ThenInclude(bp => bp.Product)// Include Category
        .ThenInclude(p => p.productImages) // Include Product Images
    .Include(s => s.BasketProducts)
        .ThenInclude(bp => bp.ProductVariation) // Include Product Variation
        .ThenInclude(pv => pv.productImages)
         .Include(s => s.BasketProducts)
        .ThenInclude(bp => bp.Product)
                    .ThenInclude(p => p.Category) // Include Category

        // Include Variation Images
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
            var ExistedProduct = await _unitOfWork.productRepository.GetEntity(s => s.Id == productId);
            if (ExistedProduct is null)
                throw new CustomException(404, "Product not found");

            ProductVariation ExistedVariation = null;

            // Check if a variation was provided
            if (variationId.HasValue)
            {
                ExistedVariation = await _unitOfWork.productVariationRepository.GetEntity(s => s.Id == variationId && s.ProductId == productId);
                if (ExistedVariation == null)
                    throw new CustomException(404, "Variation not found");
            }

            var existedBasket = await _unitOfWork.BasketRepository.GetEntity(s => s.AppUserId == user.Id, includes: new Func<IQueryable<Basket>, IQueryable<Basket>>[]
            {
        query => query.Include(c => c.BasketProducts)
                      .ThenInclude(bp => bp.Product)
                      .ThenInclude(p => p.Variations)
            });

            var BasketProduct = await _unitOfWork.BasketProductRepository.GetEntity(s =>
                s.ProductId == productId &&
                (!variationId.HasValue || s.ProductVariationId == variationId) &&
                s.Basket.AppUserId == user.Id
            );

            if (BasketProduct != null)
            {
                BasketProduct.Quantity++;
                _unitOfWork.Commit();
                return BasketProduct.Id;
            }

            if (existedBasket is not null)
            {
                existedBasket.BasketProducts.Add(new BasketProduct()
                {
                    Quantity = 1,
                    BasketId = existedBasket.Id,
                    ProductId = ExistedProduct.Id,
                    ProductVariationId = ExistedVariation?.Id  // Set the variation only if it exists
                });
                _unitOfWork.Commit();
                return existedBasket.Id;
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
                ProductId = ExistedProduct.Id,
                ProductVariationId = ExistedVariation?.Id  // Set the variation only if it exists
            };
            await _unitOfWork.BasketProductRepository.Create(newBasketProduct);
            _unitOfWork.Commit();
            return newBasketProduct.Id;
        }
    }
}
