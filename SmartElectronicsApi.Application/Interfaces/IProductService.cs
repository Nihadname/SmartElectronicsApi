using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IProductService
    {
         Task<ProductCreateDto> Create(ProductCreateDto productCreateDto);
        Task<PaginatedResponse<ProdutListItemDto>> GetAll(int pageNumber = 1, int pageSize = 10);
        Task<ProductReturnDto> GetById(int? id);
        Task<int> Delete(int? id);
    }
}
