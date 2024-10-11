using SmartElectronicsApi.Mvc.ViewModels.Color;

namespace SmartElectronicsApi.Mvc.ViewModels.ProductVariation
{
    public class ProductVariationListItemVM
    {
        public int Id { get; set; }
        public string? SKU { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int StockQuantity { get; set; }
        public string VariationName { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<ColorListItemVM> colorListItemDtos { get; set; }

    }
}
