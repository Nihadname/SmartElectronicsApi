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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<SubCategoryListItemDto> SubCategories { get; set; }
        public ICollection<ProdutListItemDto> ProdutListItemDtos { get; set; }
    }
}
