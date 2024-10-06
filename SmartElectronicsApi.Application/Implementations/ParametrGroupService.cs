using AutoMapper;
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
            try
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
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.Message, ex);
            }
        }

    }
}