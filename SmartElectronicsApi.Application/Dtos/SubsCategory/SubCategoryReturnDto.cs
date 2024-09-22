using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.SubsCategory
{
    public class SubCategoryReturnDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public CategoryInSubcategoryReturnDto  CategoryInSubcategoryReturn { get; set; }
        public List<BrandListItemDto> brandListItemDtos { get; set; }
        public List<ProdutListItemDto> produtListItemDtos { get; set; }
    }

    public class CategoryInSubcategoryReturnDto {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string ImageUrl { get; set; }

    }

}
