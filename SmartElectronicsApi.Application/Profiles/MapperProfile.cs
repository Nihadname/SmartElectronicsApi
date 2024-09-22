using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmartElectronicsApi.Api.Apps.UserInterface.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Dtos.Slider;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
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
            CreateMap<Slider, SliderReturnDto>()
                      .ForMember(s => s.Image, map => map.MapFrom(d => url + "img/" + d.Image));
            CreateMap<Slider, SliderListItemDto>()
                .ForMember(s=>s.Image,map=>map.MapFrom(d=> url+ "img/"+d.Image));
            CreateMap<SliderCreateDto, Slider>()
                .ForMember(s => s.Image, map => map.MapFrom(d =>d.Image.Save(Directory.GetCurrentDirectory(), "img")));
            CreateMap<SliderUpdateDto, Slider>()
      .ForMember(s => s.Image, map => map.MapFrom(d => d.Image.Save(Directory.GetCurrentDirectory(), "img")));
            CreateMap<CategoryCreateDto, Category>()
                .ForMember(s => s.ImageUrl, map => map.MapFrom(d => d.formFile.Save(Directory.GetCurrentDirectory(), "img")));
            CreateMap<Category, CategoryListItemDto>()
      .ForMember(dest => dest.Immage, opt => opt.MapFrom(src => src.ImageUrl))
      .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
      .ForMember(dest => dest.produtListItemDtos, opt => opt.MapFrom(src => src.Products));
      
            CreateMap<CategoryUpdateDto, Category>()
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Category, CategoryReturnDto>()
                .ForMember(s => s.Immage, map => map.MapFrom(d => url + "img/" + d.ImageUrl));
            CreateMap<SubCategoryCreateDto, SubCategory>()
                .ForMember(s => s.Image, map => map.MapFrom(d => d.formFile.Save(Directory.GetCurrentDirectory(), "img")))
  .ForMember(s => s.Brands, map => map.Ignore());
            CreateMap<SubCategory, SubCategoryListItemDto>()
                         .ForMember(s => s.Image, map => map.MapFrom(d => url + "img/" + d.Image))
                        .ForMember(s => s.brandListItemDtos, map => map.MapFrom(d => d.Brands))
                        .ForMember(s => s.produtListItemDtos, map => map.MapFrom(d => d.Products));
            CreateMap<Product, ProdutListItemDto>()
                        .ForMember(s => s.Category, map => map.MapFrom(d => d.Category));
            CreateMap<Category, CategoryInProductListItemDto>();
            CreateMap<Brand, BrandReturnDto>()
                .ForMember(s => s.ImageUrl, map => map.MapFrom(d => url + "img/" + d.ImageUrl));
            CreateMap<BrandCreateDto, Brand>()
                .ForMember(s => s.ImageUrl, map => map.MapFrom(d => d.formFile.Save(Directory.GetCurrentDirectory(), "img")));
            CreateMap<Brand, BrandListItemDto>()
                .ForMember(s => s.ImageUrl, map => map.MapFrom(d => url + "img/" + d.ImageUrl));

        }
    }
}
