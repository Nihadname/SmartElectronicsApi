﻿using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Brand
{
    public class BrandListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<SubCategoryListItemInBrandDto> SubCategoryListItemInBrandDtos { get; set; }
        public List<ProdutListItemDto> produtListItemDtos { get; set; }
    }
    
}
