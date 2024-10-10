using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Color;

namespace SmartElectronicsApi.Mvc.ViewModels.Product
{
    public class ProductCreateVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool isNew { get; set; }
        public bool IsDealOfTheWeek { get; set; }
        public bool IsFeatured { get; set; }
        public int StockQuantity { get; set; }
        public int ViewCount { get; set; }
        public decimal? DiscountedPrice { get; set; }

        public DateTime? CreatedTime { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public int SubcategoryId { get; set; }
        public List<int> ColorIds { get; set; }
        public List<IFormFile> Images { get; set; }
     


    }
}
