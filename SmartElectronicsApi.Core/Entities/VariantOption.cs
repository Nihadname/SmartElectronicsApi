using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class VariantOption
    {
        public string optionType { get; set; }
        public string optionValue { get; set; } 
         public int ProductVariationId { get; set; }
        public ProductVariation productVariation { get; set; }
    }
}
