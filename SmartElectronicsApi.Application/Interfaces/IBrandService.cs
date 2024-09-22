using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IBrandService
    {
        Task<Brand> Create(BrandCreateDto brandCreateDto);
        Task<PaginatedResponse<BrandListItemDto>> GetForAdmin(int pageNumber = 1, int pageSize = 10);
        Task<int> Delete(int? id);
        Task<BrandReturnDto> GetById(int? id);
    }
}
