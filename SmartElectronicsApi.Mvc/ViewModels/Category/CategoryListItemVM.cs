using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.SubCategory;

namespace SmartElectronicsApi.Mvc.ViewModels.Category
{
    public class CategoryListItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Immage { get; set; }
        public List<SubCategoryListItemVM> SubCategories { get; set; }
        public List<ProdutListItemVM> produtListItemDtos { get; set; }
    }
}
