using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class ProductVariationColor:BaseEntity
    {
        public int ColorId { get; set; }
        public Color Color { get; set; }
        public int ProductVariationId { get; set; }
        public ProductVariation ProductVariation { get; set; }
    }
}
