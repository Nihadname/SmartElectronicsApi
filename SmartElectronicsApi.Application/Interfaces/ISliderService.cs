using SmartElectronicsApi.Application.Dtos.Slider;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ISliderService
    {
        Task<List<SliderListItemDto>> GetAll(int take, int skip);
        Task<SliderReturnDto> GetById(int? id);
         Task<Slider> Create(SliderCreateDto sliderCreateDto);
        Task<int> Delete(int? id);
        Task<int> Update(int? id, SliderUpdateDto sliderUpdateDto);



    }
}
