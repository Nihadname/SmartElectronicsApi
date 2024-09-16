using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmartElectronicsApi.Api.Apps.UserInterface.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos.Slider;
using SmartElectronicsApi.Application.Extensions;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Profiles
{
    public class MapperProfile: Profile
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public MapperProfile(IHttpContextAccessor contextAccessor)
        {
          
            _contextAccessor = contextAccessor;
            var uriBuilder = new UriBuilder(_contextAccessor.HttpContext.Request.Scheme,
                            _contextAccessor.HttpContext.Request.Host.Host
                            , _contextAccessor.HttpContext.Request.Host.Port.Value);
            var url = uriBuilder.Uri.AbsoluteUri;
            CreateMap<AppUser, UserGetDto>();
            CreateMap<Slider, SliderReturnDto>();
            CreateMap<Slider, SliderListItemDto>();
            CreateMap<SliderCreateDto, Slider>()
                .ForMember(s => s.Image, map => map.MapFrom(d =>url+"img/" +d.Image.Save(Directory.GetCurrentDirectory(), "img")));
        }
    }
}
