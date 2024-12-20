using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class Order:BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }  
        public decimal Tax { get; set; }          
        public decimal ShippingCost { get; set; } 

        public string ShippingAddress { get; set; }  

        public OrderStatus Status { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string? ShippedToken { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }
    public enum OrderStatus
    {
        Pending,
        Completed,
        Shipped,
        Cancelled,
        Failed,
        Delivered
    }
}
