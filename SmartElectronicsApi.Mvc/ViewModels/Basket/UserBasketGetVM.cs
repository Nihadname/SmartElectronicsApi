namespace SmartElectronicsApi.Mvc.ViewModels.Basket
{
    public class UserBasketGetVM
    {
        public string UserId { get; set; }
        public List<BasketListItemVM> BasketProducts { get; set; }
        public decimal TotalPrice => BasketProducts?.Sum(bp => bp.Price * bp.Quantity) ?? 0;
        public decimal TotalSalePrice => BasketProducts?.Sum(bp => (bp.DiscountedPrice > 0 ? bp.DiscountedPrice : bp.Price) * bp.Quantity) ?? 0;
        public decimal Discount => TotalPrice - TotalSalePrice;
    }
}
