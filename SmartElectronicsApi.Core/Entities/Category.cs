﻿using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public  class Category:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string  Icon { get; set; }
          public string ImageUrl { get; set; }
        public ICollection<SubCategory>? SubCategories { get; set; }
        public ICollection<Product>? Products { get; set; }
        
    }
}
