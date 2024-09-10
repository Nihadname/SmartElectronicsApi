using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class ParametrGroup:BaseEntity
    {
public string   Name { get; set; }
        public ICollection<ParametrValue> parametrValues { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ProductVariationId { get; set; }
        public ProductVariation ProductVariation { get; set; }
    }
}
