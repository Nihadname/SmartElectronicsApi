using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ISettingService
    {
        Task<SettingDto> Create(SettingDto setting);
        Task<PaginatedResponse<SettingReturnDto>> GetForAdminUi(int pageNumber = 1, int pageSize = 10);
         Task<SettingDto> GetById(int? id);
        Task<int> Delete(int? id);
        Task<int> Update(int? id, SettingUpdateDto settingUpdateDto);
    }
}
