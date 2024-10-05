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
    }
}
