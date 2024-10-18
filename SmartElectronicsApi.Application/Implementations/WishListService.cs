using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos.Basket;
using SmartElectronicsApi.Application.Dtos.WishList;
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
    public class WishListService:IWishListService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WishListService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<UserWishListDto> GetUserBasket()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new CustomException(400, "User ID cannot be null");

            var user = await _userManager.FindByIdAsync(userId);
            var Basket = await _unitOfWork.WishListRepository.GetEntity(s => s.AppUserId == userId, includes: new Func<IQueryable<WishList>, IQueryable<WishList>>[] {
query => query
    .Include(s => s.wishListProducts)
        .ThenInclude(bp => bp.Product)
        .ThenInclude(p => p.productImages)
    .Include(s => s.wishListProducts)
        .ThenInclude(bp => bp.ProductVariation)
        .ThenInclude(pv => pv.productImages)
         .Include(s => s.wishListProducts)
        .ThenInclude(bp => bp.Product)
                    .ThenInclude(p => p.Category)

        });
            if (Basket == null) throw new CustomException(404, " Not found.");
            var userBasketDto = _mapper.Map<UserWishListDto>(Basket);
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
            if (user == null)
                throw new CustomException(404, "User not found");

            var existedProduct = await _unitOfWork.productRepository.GetEntity(s => s.Id == productId);
            if (existedProduct == null)
                throw new CustomException(404, "Product not found");

            ProductVariation existedVariation = null;
            if (variationId.HasValue)
            {
                existedVariation = await _unitOfWork.productVariationRepository.GetEntity(s => s.Id == variationId && s.ProductId == productId);
                if (existedVariation == null)
                    throw new CustomException(404, "Variation not found");
            }

            var existedWishList = await _unitOfWork.WishListRepository.GetEntity(s => s.AppUserId == user.Id, includes: new Func<IQueryable<WishList>, IQueryable<WishList>>[]
            {
        query => query.Include(c => c.wishListProducts)
            });

            if (existedWishList != null)
            {
                bool isProductInWishList = existedWishList.wishListProducts.Any(wp => wp.ProductId == productId && wp.ProductVariationId == variationId);
                if (!isProductInWishList)
                {
                    existedWishList.wishListProducts.Add(new WishListProduct()
                    {
                        CreatedTime = DateTime.Now,
                        WishListId = existedWishList.Id,
                        ProductId = existedProduct.Id,
                        ProductVariationId = existedVariation?.Id
                    });
                }
            }
            else
            {
                var newWishList = new WishList()
                {
                    AppUserId = user.Id,
                    wishListProducts = new List<WishListProduct>()
            {
                new WishListProduct()
                {
                    CreatedTime = DateTime.Now,
                    ProductId = existedProduct.Id,
                    ProductVariationId = existedVariation?.Id
                }
            }
                };
                await _unitOfWork.WishListRepository.Create(newWishList);
                existedWishList = newWishList;  
            }

             _unitOfWork.Commit();

            return existedWishList.wishListProducts.Count();
        }


        public async Task<int> Delete(int? productId, int? variationId = null)
        {
            if (productId == null)
                throw new CustomException(400, "ProductId cannot be null");

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new CustomException(400, "User ID cannot be null");

            var user = await _userManager.FindByIdAsync(userId);
            var WishListProduct = await _unitOfWork.WishListProductRepository.GetEntity(s =>
                s.ProductId == productId &&
                (variationId.HasValue ? s.ProductVariationId == variationId : s.ProductVariationId == null) &&
                s.WishList.AppUserId == user.Id
            );
            if (WishListProduct == null)
                throw new CustomException(404, "Product not found in the basket");

            await _unitOfWork.WishListProductRepository.Delete(WishListProduct);
            _unitOfWork.Commit();
            var existedWishList = await _unitOfWork.WishListRepository.GetEntity(s => s.AppUserId == user.Id, includes: new Func<IQueryable<WishList>, IQueryable<WishList>>[]
           {
        query => query.Include(c => c.wishListProducts)
           });
            return existedWishList.wishListProducts.Count();

        }
        public async Task<List<int?>> GetUserWishListProducts()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new CustomException(400, "User ID cannot be null");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new CustomException(404, "User not found");
            var productIds = (await _unitOfWork.WishListProductRepository.GetAll(wp => wp.WishList.AppUserId == userId)).Select(s=>s.ProductId).ToList();
            return productIds;
        }
    }
}
