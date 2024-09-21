using AutoMapper;
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
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public SubCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SubCategory> Create(SubCategoryCreateDto subCategoryCreateDto)
        {
            if(!await _unitOfWork.subCategoryRepository.isExists(s => s.Id == subCategoryCreateDto.CategoryId))
            {
                throw new CustomException(400, "CategoryId", "this Subcategory doesnt exist");
            }
            if(await _unitOfWork.subCategoryRepository.isExists(s => s.Name.ToLower() == subCategoryCreateDto.Name.ToLower()))
            {
                throw new CustomException(400, "Name", "this Subcategory name already exists");

            }
            var subCategory= _mapper.Map<SubCategory>(subCategoryCreateDto);
            await _unitOfWork.subCategoryRepository.Create(subCategory);
            _unitOfWork.Commit();
         return subCategory;
        }
    }
}
