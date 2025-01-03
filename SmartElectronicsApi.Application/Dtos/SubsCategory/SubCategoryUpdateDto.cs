﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.SubsCategory
{
    public class SubCategoryUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? formFile { get; set; }
        public List<int>? BrandIds { get; set; }
    }
}
