﻿namespace SmartElectronicsApi.Mvc.ViewModels.Category
{
    public class CategoryUpdateVM
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public IFormFile? formFile { get; set; }
    }
}