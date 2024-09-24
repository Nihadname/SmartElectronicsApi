using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.Product;

namespace SmartElectronicsApi.Mvc.ViewModels.SubCategory
{
    public class SubCategoryListItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<BrandListItemVM> brandListItemDtos { get; set; }
        public List<ProdutListItemVM> produtListItemDtos { get; set; }
    }
}
