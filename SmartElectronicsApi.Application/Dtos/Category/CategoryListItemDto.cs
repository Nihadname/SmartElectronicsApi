using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Category
{
    public class CategoryListItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Immage { get; set; }
      public List<SubCategoryListItemDto> SubCategories { get; set; }
        public List<ProdutListItemDto> produtListItemDtos { get; set; }
    }
}
