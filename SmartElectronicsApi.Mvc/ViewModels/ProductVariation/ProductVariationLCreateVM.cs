namespace SmartElectronicsApi.Mvc.ViewModels.ProductVariation
{
    public class ProductVariationLCreateVM
    {
        public string? SKU { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int StockQuantity { get; set; }
        public string VariationName { get; set; }
        public int ProductId { get; set; }
        public List<int> ColorIds { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
