using SmartElectronicsApi.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Order
{
    public class OrderAdminListItemDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public UserInOrderDto User { get; set; }
    }
    public class UserInOrderDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
