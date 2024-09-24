using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Slider;

namespace SmartElectronicsApi.Mvc.ViewModels
{
    public class HomeViewModel
    {
        public List<SliderListItemVm> Sliders { get; set; }
        public List<CategoryListItemVM> Categories { get; set; }
    }
}
