using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.ProductVariation
{
    public class ProductVariationCreateDto
    {
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int StockQuantity { get; set; }
        public string VariationName { get; set; }
        public int ProductId { get; set; }
        public List<int> ColorIds { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
