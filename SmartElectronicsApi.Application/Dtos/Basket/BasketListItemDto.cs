using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Basket
{
    public class BasketListItemDto
    {
        public int? ProductId { get; set; }
        public int? VariationId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; } 
        public decimal DiscountedPrice { get; set; } 
        public int Quantity { get; set; } 
    }
}
