namespace SmartElectronicsApi.Mvc.ViewModels.Brand
{
    public class BrandCreateVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile formFile { get; set; }
    }
}
