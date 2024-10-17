namespace SmartElectronicsApi.Mvc.ViewModels.WishList
{
    public class ProductWishListItemVM
    {
        public int ProductId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
