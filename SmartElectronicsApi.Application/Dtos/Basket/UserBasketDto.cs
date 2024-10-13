using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Basket
{
    public class UserBasketDto
    {
        public string UserId { get; set; }
       
        public List<BasketListItemDto> BasketProducts { get; set; }
        public decimal TotalPrice => BasketProducts?.Sum(bp => bp.Price * bp.Quantity) ?? 0;
        public decimal TotalSalePrice => BasketProducts?.Sum(bp => (bp.DiscountedPrice > 0 ? bp.DiscountedPrice : bp.Price) * bp.Quantity) ?? 0;
        public decimal Discount => TotalPrice - TotalSalePrice;
        public int Count {  get; set; }
    }
}
