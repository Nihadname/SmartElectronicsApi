using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.SubCategory;

namespace SmartElectronicsApi.Mvc.ViewModels.Brand
{
    public class BrandListItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<SubCategoryInBrandListItemDto> SubCategoryListItemInBrandDtos { get; set; }
        public List<ProdutListItemVM> produtListItemDtos { get; set; }
    }
    public class SubCategoryInBrandListItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
