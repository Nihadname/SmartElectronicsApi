using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        public BasketService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Add(int? productId)
        {
            if (productId == null) throw new CustomException(400, "productId can not be null");
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");
            }
            var user = await _userManager.FindByIdAsync(userId);
            var ExistedProduct=await _unitOfWork.productRepository.GetEntity(s=>s.Id==productId);
            if(ExistedProduct is null) throw new CustomException(404, "Not found");
            var existedBasket = await _unitOfWork.BasketRepository.GetEntity(s => s.AppUserId == user.Id, includes: new Func<IQueryable<Basket>, IQueryable<Basket>>[]
                {
        query => query.Include(c => c.BasketProducts)
                   
                      //.ThenInclude(sc => sc.brandSubCategories) // Include BrandSubCategories
                      //.ThenInclude(bsc => bsc.Brand)
                      //.ThenInclude(s=>s.Products)
                });
            var BasketProduct=await _unitOfWork.BasketProductRepository.GetEntity(s=>s.ProductId==productId&&s.Basket.AppUserId==user.Id);
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
            BasketProduct basketProduct = new()
            {
                Quantity = 1,
                BasketId = newBasket.Id,
                ProductId = ExistedProduct.Id,
            };
            await _unitOfWork.BasketProductRepository.Create(basketProduct);
            _unitOfWork.Commit();
            return basketProduct.Id;
        }
    }
}
