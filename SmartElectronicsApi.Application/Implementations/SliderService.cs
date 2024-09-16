using AutoMapper;
using SmartElectronicsApi.Application.Dtos.Slider;
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
    public class SliderService : ISliderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SliderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<Slider> Create()
        {
            throw new NotImplementedException();
        }

        public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var Slider = await _unitOfWork.sliderRepository.GetEntity(s => s.Id == id);
            if (Slider is null) throw new CustomException(404, "Not found");
            await _unitOfWork.sliderRepository.Delete(Slider);
             _unitOfWork.Commit();
            return Slider.Id;
        }

        public async Task<List<SliderListItemDto>> GetAll()
        {
            var sliders = await _unitOfWork.sliderRepository.GetAll();
            var sliderItemDto = _mapper.Map<List<SliderListItemDto>>(sliders); // Correctly map the list
            return sliderItemDto;
        }

        public async Task<SliderReturnDto> GetById(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var Slider=await _unitOfWork.sliderRepository.GetEntity(s=>s.Id==id);
            if (Slider is null) throw new CustomException(404, "Not found");
            var slider=_mapper.Map<SliderReturnDto>(Slider);
            return slider;
        }

        public Task<Slider> Update()
        {
            throw new NotImplementedException();
        }
    }
}
