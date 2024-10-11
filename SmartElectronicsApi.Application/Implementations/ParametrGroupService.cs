using AutoMapper;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Color;
using SmartElectronicsApi.Application.Dtos.ParametrGroup;
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
    public class ParametrGroupService : IParametrGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ParametrGroupService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Create(ParametrGroupCreateDto parametrGroupCreateDto)
        {
           

                if (!await _unitOfWork.productRepository.isExists(s => s.Id == parametrGroupCreateDto.ProductId))
                {
                    throw new CustomException(400, "Invalid product id");
                }

                var entity = _mapper.Map<ParametrGroup>(parametrGroupCreateDto);

               

                await _unitOfWork.parametricGroupRepository.Create(entity);

                 _unitOfWork.Commit();
                foreach (var item in parametrGroupCreateDto.parametrValues)
                {
                    var mappedValue = _mapper.Map<ParametrValue>(item);
                    mappedValue.ParametrGroupId=entity.Id;
                    entity.parametrValues.Add(mappedValue);
                    await _unitOfWork.parametrValueRepository.Create(mappedValue);
                    _unitOfWork.Commit();
                }
                return entity.Id;
            }
         public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var ParametrGroup = await _unitOfWork.parametricGroupRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (ParametrGroup is null) throw new CustomException(404, "Not found");
            await _unitOfWork.parametricGroupRepository.Delete(ParametrGroup);
            _unitOfWork.Commit();
            return ParametrGroup.Id;
        }
        public async Task<ParametrGroupListItemDto> GetById(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var ParametrGroup = await _unitOfWork.parametricGroupRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (ParametrGroup is null) throw new CustomException(404, "Not found");
            var MappedValue=_mapper.Map<ParametrGroupListItemDto>(ParametrGroup);
            return MappedValue;
        }
        public async Task<PaginatedResponse<ParametrGroupListItemDto>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var totalCount = (await _unitOfWork.parametricGroupRepository.GetAll()).Count();
            var ParametrGroup = await _unitOfWork.parametricGroupRepository.GetAll(s=>s.IsDeleted == false);
            var MappedValue = _mapper.Map<List<ParametrGroupListItemDto>>(ParametrGroup);
            return new PaginatedResponse<ParametrGroupListItemDto>
            {
                Data = MappedValue,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

    }

    }
