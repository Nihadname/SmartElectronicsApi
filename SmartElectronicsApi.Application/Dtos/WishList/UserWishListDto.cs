using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.WishList
{
    public class UserWishListDto
    {
        public string UserId { get; set; }
        public ICollection<WishListProductListItemDto> wishListProducts { get; set; }
        public int Count { get; set; }
    }
}
