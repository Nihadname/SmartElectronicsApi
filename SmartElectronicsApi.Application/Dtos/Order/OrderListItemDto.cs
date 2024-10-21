using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Order
{
    public class OrderListItemDto
    {
        public int Id { get; set; }           
        public DateTime OrderDate { get; set; }      
        public decimal TotalAmount { get; set; }     
        public OrderStatus Status { get; set; }      
        public string ShippingAddress { get; set; } 


        public List<OrderItemSummaryDto> Items { get; set; } = new List<OrderItemSummaryDto>();
    }
    public class OrderItemSummaryDto
    {
        public int ProductId { get; set; }             // Product ID for the item
        public string ProductName { get; set; }        // Name of the product
        public int Quantity { get; set; }              // Quantity ordered
        public decimal UnitPrice { get; set; }         // Price per unit, considering variation
        public int? ProductVariationId { get; set; }   // Optional Product Variation ID
        public string? VariationName { get; set; }
    }
}
