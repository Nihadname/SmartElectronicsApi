using System.ComponentModel;

namespace SmartElectronicsApi.Mvc.ViewModels.Order
{
    public class OrderVerfiyVM
    {
        public string UserName { get; set; }
        [DisplayName("OrderId")]
        public int Id { get; set; }
        public string ShippedToken { get; set; }
    }
}
