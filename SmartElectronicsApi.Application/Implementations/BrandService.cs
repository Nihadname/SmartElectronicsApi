using AutoMapper;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
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
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public BrandService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Brand> Create(BrandCreateDto brandCreateDto)
        {
            if (await _unitOfWork.brandRepository.isExists(s => s.Name.ToLower() == brandCreateDto.Name.ToLower()))
            {
                throw new CustomException(400, "Name", "this Brand name already exists");
            }
            var brand = _mapper.Map<Brand>(brandCreateDto);
            await _unitOfWork.brandRepository.Create(brand);
            _unitOfWork.Commit();
            return brand;

        }
    }
}
