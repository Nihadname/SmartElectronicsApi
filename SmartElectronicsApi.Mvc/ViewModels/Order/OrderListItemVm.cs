namespace SmartElectronicsApi.Mvc.ViewModels.Order
{
    public class OrderListItemVm
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }


        public List<OrderItemSummaryDto> Items { get; set; } = new List<OrderItemSummaryDto>();
    }
    public class OrderItemSummaryDto
    {
        public int ProductId { get; set; }             
        public string ProductName { get; set; }        
        public int Quantity { get; set; }            
        public decimal UnitPrice { get; set; }        
        public int? ProductVariationId { get; set; }   

    }
}
