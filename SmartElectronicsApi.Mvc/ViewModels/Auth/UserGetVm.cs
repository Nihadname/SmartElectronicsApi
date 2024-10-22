using SmartElectronicsApi.Mvc.ViewModels.Order;

namespace SmartElectronicsApi.Mvc.ViewModels.Auth
{
    public class UserGetVm
    {
        public string FullName { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public bool IsBlocked { get; set; }

        public DateTime? CreatedTime { get; set; }
        public int loyalPoints { get; set; }
        public int loyaltyTier { get; set; }
        public ICollection<OrderListItemVm>? orders { get; set; }
        public int OrdersCount { get; set; }
        public int WishListedItemsCount { get; set; }
        public int TotalAmountSum { get; set; }
        public List<string> FavoriteCategories { get; set; }
    }
}
