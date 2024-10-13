namespace SmartElectronicsApi.Mvc.ViewModels.Basket
{
    public class BasketListItemVM
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int Quantity { get; set; }
    }
}
