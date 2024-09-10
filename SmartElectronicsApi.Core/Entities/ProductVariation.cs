using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class ProductVariation:BaseEntity
    {
        public string SKU { get; set; }  
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int StockQuantity { get; set; }
        public string VariationName { get; set; } 
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public ICollection<ProductVariationColor> productVariationColors { get; set; }
        public ICollection<ProductImage> productImages { get; set; }
        public ICollection<ParametrGroup> productParametrGroups { get; set; }

    }
}
