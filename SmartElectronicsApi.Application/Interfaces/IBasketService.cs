using SmartElectronicsApi.Application.Dtos.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IBasketService
    {
        Task<int> Add(int? productId, int? variationId);
        Task<UserBasketDto> GetUserBasket();
        Task<int> ChangeQuantity(int? productId, int? variationId = null, int quantityChange = 1);
    }
}
