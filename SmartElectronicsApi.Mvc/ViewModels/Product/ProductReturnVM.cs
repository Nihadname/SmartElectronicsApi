using SmartElectronicsApi.Mvc.ViewModels.Color;
using SmartElectronicsApi.Mvc.ViewModels.ParametrGroup;
using SmartElectronicsApi.Mvc.ViewModels.ProductVariation;

namespace SmartElectronicsApi.Mvc.ViewModels.Product
{
    public class ProductReturnVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int ViewCount { get; set; }
        public bool isNew { get; set; }
        public string ProductCode { get; set; }
        public bool IsDealOfTheWeek { get; set; }
        public bool IsFeatured { get; set; }
        public int StockQuantity { get; set; }
        public CategoryInProductListItemVm Category { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<ColorListItemVM> colorListItemDtos { get; set; }
        public List<ParametrGroupListItemVM> parametrGroupListItemDtos { get; set; }
        public List<ProductVariationListItemVM> productVariationListItemDtos { get; set; }

    }
}
