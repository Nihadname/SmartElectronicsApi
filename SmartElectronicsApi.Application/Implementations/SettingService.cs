using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Dtos.Setting;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class SettingService:ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public SettingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<SettingDto> Create(SettingDto setting)
        {
            var MappedSetting=_mapper.Map<Setting>(setting);
            await _unitOfWork.settingRepository.Create(MappedSetting);
            _unitOfWork.Commit();
            return setting;
        }
        public async Task<PaginatedResponse<SettingReturnDto>> GetForAdminUi(int pageNumber = 1, int pageSize = 10)
        {
            var TotalCount = (await _unitOfWork.settingRepository.GetAll()).Count();
            var settings=await _unitOfWork.settingRepository.GetAll(s => s.IsDeleted == false, (pageNumber - 1) * pageSize,
                                                                  pageSize);  
            var settingsMapped=_mapper.Map<IEnumerable<SettingReturnDto>>(settings);
            return new PaginatedResponse<SettingReturnDto >
            {
                Data = settingsMapped,
                TotalRecords = TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };


        }
        public async Task<SettingDto> GetById(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var setting = await _unitOfWork.settingRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (setting is null) throw new CustomException(404, "Not found");
            var MappedSetting=_mapper.Map<SettingDto>(setting);
            return MappedSetting;
        }
        public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var setting = await _unitOfWork.settingRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (setting is null) throw new CustomException(404, "Not found");
            await _unitOfWork.settingRepository.Delete(setting);
            _unitOfWork.Commit();
            return setting.Id;
        }
        public async Task<int> Update(int? id, SettingUpdateDto settingUpdateDto)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var setting = await _unitOfWork.settingRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (setting is null) throw new CustomException(404, "Not found");
            _mapper.Map(settingUpdateDto, setting);
            await _unitOfWork.settingRepository.Update(setting);
            _unitOfWork.Commit();
            return setting.Id;
        }
    }
}
