using SmartElectronicsApi.Application.Dtos.WishList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IWishListService
    {
        Task<int> Add(int? productId, int? variationId = null);
        Task<int> Delete(int? productId, int? variationId = null);
        Task<UserWishListDto> GetUserBasket();
        Task<List<int?>> GetUserWishListProducts();
    }
}
