using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.ProductVariation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IProductVariationService
    {
        Task<int> Delete(int? id);
        Task<ProductVariationCreateDto> Create(ProductVariationCreateDto productVariationCreateDto);
        Task<PaginatedResponse<ProductVariationListItemDto>> GetAll(
         int pageNumber = 1,
         int pageSize = 10);
    }
}
