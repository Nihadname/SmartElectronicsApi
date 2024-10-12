namespace SmartElectronicsApi.Mvc.ViewModels.SubCategory
{
    public class SubCategoryUpdateVM
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? formFile { get; set; }
        public List<int>? BrandIds { get; set; }
    }
}
