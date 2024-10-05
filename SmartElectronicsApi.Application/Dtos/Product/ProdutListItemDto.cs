using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Product
{
    public class ProdutListItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ViewCount { get; set; }
        public CategoryInProductListItemDto Category { get; set; }


    }
    public class CategoryInProductListItemDto  
    {
        public string Name { get; set; }
        public int ProductCount { get; set; }
    }
}
