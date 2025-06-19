using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Order
{
    public class OrderReturnDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        
        public List<OrderItemSummaryDto> Items { get; set; }
    }
    public sealed record UserOrderReturnDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemSummaryDto> Items { get; set; }
        public UserOrderSummary UserOrder { get; set; }
        
       
    }

    public sealed record UserOrderSummary
    {
        public string FullName { get; init; }
        public string Email { get; init; }
        
    }
}
