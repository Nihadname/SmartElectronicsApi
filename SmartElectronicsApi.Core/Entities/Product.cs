using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class Product:BaseEntity
    {
    public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public bool isNew { get; set; }
        public string ProductCode { get; set; }
        public bool IsDealOfTheWeek  { get; set; }
        public bool IsFeatured { get; set; }
        public int StockQuantity { get; set; }
        public int ViewCount { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
        public int BrandId  { get; set; }
        public Brand Brand { get; set; }
        public ICollection<ProductVariation> Variations { get; set; }
        public ICollection<ProductColor> productColors { get; set; }
        public ICollection<ProductImage> productImages { get; set; }
        public ICollection<ParametrGroup> parametricGroups { get; set; }
        public ICollection<BasketProduct>? BasketProducts { get; set; }
        public ICollection<WishListProduct>? WishListProducts { get;set; }
        public ICollection<Comment> comments { get; set; }

    }
}
