﻿using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class Color:BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public ICollection<ProductVariationColor> productVariationColors { get; set; }
        public ICollection<ProductColor> productColorColors { get; set; }
    }
}
