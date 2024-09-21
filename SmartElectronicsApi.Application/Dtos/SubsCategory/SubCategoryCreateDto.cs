using Microsoft.AspNetCore.Http;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.SubsCategory
{
    public class SubCategoryCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public IFormFile formFile { get; set; }
        public List<Brand> brands { get; set; }
    }
}
