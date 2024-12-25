using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class GuestOrder:BaseEntity
    {
        public string FullName { get; set; }
        public int Age {  get; set; }
        public string Address { get; set; }
        public string EmailAdress { get; set; }
        public string PhoneNumber { get; set; }
        public string ExtraInformation { get; set; }
        public bool IsGottenFromStore { get; set; }
        public int? PurchasedProductId {  get; set; }
        public int? PurchasedProducVariationtId { get; set; }
        public string ProductName { get; set; }  
        public decimal ProductPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
