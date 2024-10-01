using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Color;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IColorService
    {
        Task<ColorCreateDto> Create(ColorCreateDto colorCreateDto);
        Task<int> Delete(int? id);
        Task<PaginatedResponse<ColorListItemDto>> GetAllForAdminUi(int pageNumber = 1, int pageSize = 10);
        Task<ColorListItemDto> GetById(int? id);
        Task<Color> Update(int? id, ColorUpdateDto colorUpdateDto);
    }
}
