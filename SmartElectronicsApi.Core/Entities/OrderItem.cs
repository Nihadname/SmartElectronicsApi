﻿using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class OrderItem:BaseEntity
    {
    

        public int OrderId { get; set; } 
        public Order Order { get; set; } 

        public int ProductId { get; set; } 
        public Product Product { get; set; }
        public int? ProductVariationId { get; set; }
        public ProductVariation? ProductVariation { get; set; }
        public int Quantity { get; set; }  
        public decimal UnitPrice { get; set; } 
        public decimal TotalPrice => UnitPrice * Quantity; 
    }
}
