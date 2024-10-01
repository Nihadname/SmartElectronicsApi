using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.Color;
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
    public class ColorService : IColorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ColorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ColorCreateDto> Create(ColorCreateDto colorCreateDto)
        {
            if (await _unitOfWork.colorRepository.isExists(s => s.Code.ToLower() == colorCreateDto.Code.ToLower()))
            {
                throw new CustomException(400, "Code", "they cant have the same colors already existed one");
            }
            var MappedOne = _mapper.Map<Color>(colorCreateDto);
            await _unitOfWork.colorRepository.Create(MappedOne);
            _unitOfWork.Commit();
            return colorCreateDto;
        }
        public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var color = await _unitOfWork.colorRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (color is null) throw new CustomException(404, "Not found");
            await _unitOfWork.colorRepository.Delete(color);
            _unitOfWork.Commit();
            return color.Id;
        }
        public async Task<PaginatedResponse<ColorListItemDto>> GetAllForAdminUi(int pageNumber = 1, int pageSize = 10)
        {
            var totalCount = (await _unitOfWork.colorRepository.GetAll()).Count();

            var colors = await _unitOfWork.colorRepository.GetAll(s => s.IsDeleted == false,
                                                                  (pageNumber - 1) * pageSize,
                                                                  pageSize);

            var colorsWithMapping = _mapper.Map<List<ColorListItemDto>>(colors);

            return new PaginatedResponse<ColorListItemDto>
            {
                Data = colorsWithMapping,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<ColorListItemDto> GetById(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var color = await _unitOfWork.colorRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (color is null) throw new CustomException(404, "Not found");
            var MappedColor = _mapper.Map<ColorListItemDto>(color);
            return MappedColor;
        }
        public async Task<Color> Update(int? id, ColorUpdateDto colorUpdateDto)
        {
            if (id is null) throw new CustomException(400, "Id", "id can't be null");

            var color = await _unitOfWork.colorRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (color is null) throw new CustomException(404, "Not found");

            if (!string.IsNullOrEmpty(colorUpdateDto.Code))
            {
                if (await _unitOfWork.colorRepository.isExists(s => s.Code.ToLower() == colorUpdateDto.Code.ToLower()))
                {
                    throw new CustomException(400, "Code", "A color with the same code already exists.");
                }
            }

            color.Name = colorUpdateDto.Name ?? color.Name;
            color.Code = colorUpdateDto.Code ?? color.Code;

            await _unitOfWork.colorRepository.Update(color);
             _unitOfWork.Commit();

            return color;
        }

    }
}