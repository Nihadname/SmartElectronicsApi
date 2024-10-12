using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.SubCategory;

namespace SmartElectronicsApi.Mvc.ViewModels
{
    public class BrandDetailVM
    {
        public BrandListItemVM BrandListItem { get; set; }
        public List<ProdutListItemVM> produtListItemVMs { get; set; }
        public List<ProdutListItemVM> produtListItemVMsWithDealOfTheWeek { get; set; }
    }
}
