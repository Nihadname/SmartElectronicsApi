using AutoMapper;
using SmartElectronicsApi.Application.Dtos.Slider;
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
  
        public class SliderService : ISliderService
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            public SliderService(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<Slider> Create(SliderCreateDto sliderCreateDto)
            {
                var Slider = _mapper.Map<Slider>(sliderCreateDto);

                await _unitOfWork.sliderRepository.Create(Slider);
                _unitOfWork.Commit();
                return Slider;
            }

            public async Task<int> Delete(int? id)
            {
                if (id is null) throw new CustomException(400, "Id", "id cant be null");
                var Slider = await _unitOfWork.sliderRepository.GetEntity(s => s.Id == id);
                if (Slider is null) throw new CustomException(404, "Not found");
            if (!string.IsNullOrEmpty(Slider.Image))
            {
                Slider.Image.DeleteFile();
            }
            await _unitOfWork.sliderRepository.Delete(Slider);
                _unitOfWork.Commit();
                return Slider.Id;
            }
        public async Task<List<SliderListItemDto>> GetAll()
        {
            var sliders = await _unitOfWork.sliderRepository.GetAll(s => s.IsDeleted == false);
            var sliderItemDto = _mapper.Map<List<SliderListItemDto>>(sliders); // Correctly map the list
            return sliderItemDto;
        }
        public async Task<List<SliderListItemDto>> GetAll(int skip, int take)
            {
                var sliders = await _unitOfWork.sliderRepository.GetAll(s=>s.IsDeleted==false,skip,take);
                var sliderItemDto = _mapper.Map<List<SliderListItemDto>>(sliders); // Correctly map the list
                return sliderItemDto;
            }

            public async Task<SliderReturnDto> GetById(int? id)
            {
                if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var Slider = await _unitOfWork.sliderRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (Slider is null) throw new CustomException(404, "Not found");
                var slider = _mapper.Map<SliderReturnDto>(Slider);
                return slider;
            }

            public async Task<int> Update(int? id, SliderUpdateDto sliderUpdateDto)
            {
                if (id is null) throw new CustomException(400, "Id", "id can't be null");

                var slider = await _unitOfWork.sliderRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
                if (slider is null) throw new CustomException(404, "Slider not found");

                if (sliderUpdateDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(slider.Image))
                    {
                        slider.Image.DeleteFile();
                    }
                    var newImagePath = sliderUpdateDto.Image.Save(Directory.GetCurrentDirectory(), "img");
                    slider.Image = newImagePath;
                   var UpdatedSlider= _mapper.Map(sliderUpdateDto, slider);

                    _unitOfWork.sliderRepository.Update(UpdatedSlider);
                     _unitOfWork.Commit();

                    return UpdatedSlider.Id;
                }
                else
                {
                    return 0;
                }

                
            }
        
        }
    }
