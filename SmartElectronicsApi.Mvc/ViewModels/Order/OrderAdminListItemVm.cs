namespace SmartElectronicsApi.Mvc.ViewModels.Order
{
    public class OrderAdminListItemVM
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public UserInOrderVm User { get; set; }
    }
    public class UserInOrderVm
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
