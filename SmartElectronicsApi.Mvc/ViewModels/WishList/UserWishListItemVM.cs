namespace SmartElectronicsApi.Mvc.ViewModels.WishList
{
    public class UserWishListItemVM
    {
        public string UserId { get; set; }
        public ICollection<ProductWishListItemVM> wishListProducts { get; set; }
        public int Count { get; set; }

    }
}
