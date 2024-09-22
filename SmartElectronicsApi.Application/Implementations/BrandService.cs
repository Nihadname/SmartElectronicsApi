using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Extensions;
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
        public async Task<PaginatedResponse<BrandListItemDto>> GetForAdmin(int pageNumber = 1, int pageSize = 10)
        {
            var TotalCount = (await _unitOfWork.brandRepository.GetAll()).Count();
            var Brands=await _unitOfWork.brandRepository.GetAll(s => s.IsDeleted == false, (pageNumber - 1) * pageSize, pageSize, includes: new Func<IQueryable<Brand>, IQueryable<Brand>>[]
    {
        query => query.Include(s=>s.SubCategory).Include(p => p.Products)
    }
);
            var brandsWithMapping=_mapper.Map<List<BrandListItemDto>>(Brands);
            return new PaginatedResponse<BrandListItemDto> {
                Data = brandsWithMapping,
                TotalRecords = TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }
        public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var brand = await _unitOfWork.brandRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (brand is null) throw new CustomException(404, "Not found");
            if (!string.IsNullOrEmpty(brand.ImageUrl))
            {
                brand.ImageUrl.DeleteFile();
            }
            await _unitOfWork.brandRepository.Delete(brand);
            _unitOfWork.Commit();
            return brand.Id;
        }
        public async Task<BrandReturnDto> GetById(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var brand = await _unitOfWork.brandRepository.GetEntity(s => s.Id == id && s.IsDeleted == false, includes: new Func<IQueryable<Brand>, IQueryable<Brand>>[]
    {
        query => query.Include(s=>s.Products)
    }
);
            if (brand is null) throw new CustomException(404, "Not found");
            var brandMapping=_mapper.Map<BrandReturnDto>(brand);
            return brandMapping;
        }
    }
}
