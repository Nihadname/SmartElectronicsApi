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
    }
}
