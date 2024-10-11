using SmartElectronicsApi.Application.Dtos.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.ProductVariation
{
    public class ProductVariationListItemDto
    {
        public int Id { get; set; }
        public string? SKU { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int StockQuantity { get; set; }
        public string VariationName { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<ColorListItemDto> colorListItemDtos { get; set; }
    }
}
