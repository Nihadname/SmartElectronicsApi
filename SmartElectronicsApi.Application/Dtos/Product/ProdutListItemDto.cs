using SmartElectronicsApi.Application.Dtos.Color;
using SmartElectronicsApi.Application.Dtos.Comment;
using SmartElectronicsApi.Application.Dtos.ParametrGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Product
{
    public class ProdutListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int ViewCount { get; set; }
        public bool isNew { get; set; }
        public string ProductCode { get; set; }
        public bool IsDealOfTheWeek { get; set; }
        public bool IsFeatured { get; set; }
        public int StockQuantity { get; set; }
        public int BrandId { get; set; }
        public CategoryInProductListItemDto Category { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<ColorListItemDto> colorListItemDtos { get; set; }
        public List<ParametrGroupListItemDto> parametrGroupListItemDtos { get; set; }
        public List<CommentListItemDto>? commentListItemDtos { get; set; }

    }
    public class CategoryInProductListItemDto  
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductCount { get; set; }
    }
   
}
