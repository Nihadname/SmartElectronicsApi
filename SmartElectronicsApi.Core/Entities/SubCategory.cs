using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class SubCategory:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image {  get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<BrandSubCategory> brandSubCategories { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
