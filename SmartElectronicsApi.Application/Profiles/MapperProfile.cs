using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmartElectronicsApi.Api.Apps.UserInterface.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos.Address;
using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Dtos.Color;
using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Dtos.Setting;
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
    public class MapperProfile : Profile
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public MapperProfile(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            var uriBuilder = new UriBuilder(_contextAccessor.HttpContext.Request.Scheme,
                            _contextAccessor.HttpContext.Request.Host.Host,
                            _contextAccessor.HttpContext.Request.Host.Port.Value);
            var url = uriBuilder.Uri.AbsoluteUri;
            var configuration = new MapperConfiguration(cfg =>
            {

          
            // User mappings
            CreateMap<AppUser, UserGetDto>()
                 .ForMember(s => s.PhoneNumber, map => map.MapFrom(d => d.PhoneNumber))
                  .ForMember(s => s.Image, map => map.MapFrom(d => url + "img/" + d.Image));
                CreateMap<UpdateUserDto, AppUser>()
                 .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

                // Slider mappings
                CreateMap<Slider, SliderReturnDto>()
                .ForMember(s => s.Image, map => map.MapFrom(d => url + "img/" + d.Image));
            CreateMap<Slider, SliderListItemDto>()
                .ForMember(s => s.Image, map => map.MapFrom(d => url + "img/" + d.Image));
            CreateMap<SliderCreateDto, Slider>()
                .ForMember(s => s.Image, map => map.MapFrom(d => d.Image.Save(Directory.GetCurrentDirectory(), "img")));
            CreateMap<SliderUpdateDto, Slider>()
                .ForMember(s => s.Image, map => map.MapFrom(d => d.Image.Save(Directory.GetCurrentDirectory(), "img")));

            // Category mappings
            CreateMap<CategoryCreateDto, Category>()
                .ForMember(s => s.ImageUrl, map => map.MapFrom(d => d.formFile.Save(Directory.GetCurrentDirectory(), "img")));
            CreateMap<Category, CategoryListItemDto>()
                .ForMember(s => s.ImageUrl, map => map.MapFrom(d => url + "img/" + d.ImageUrl))
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
                .ForMember(dest => dest.produtListItemDtos, opt => opt.MapFrom(src => src.Products));
            CreateMap<CategoryUpdateDto, Category>()
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Category, CategoryReturnDto>()
                .ForMember(s => s.Immage, map => map.MapFrom(d => url + "img/" + d.ImageUrl))
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
                .ForMember(dest => dest.produtListItemDtos, opt => opt.MapFrom(src => src.Products));
            CreateMap<Category, CategoryInSubcategoryReturnDto>();

            // SubCategory mappings
            CreateMap<SubCategoryCreateDto, SubCategory>()
                .ForMember(s => s.Image, map => map.MapFrom(d => d.formFile.Save(Directory.GetCurrentDirectory(), "img")));
            CreateMap<SubCategory, SubCategoryListItemDto>()
       .ForMember(dest => dest.Image, opt => opt.MapFrom(src => url + "img/" + src.Image))
       .ForMember(dest => dest.produtListItemDtos, opt => opt.MapFrom(src => src.Products))
       .ForMember(dest => dest.brandListItemDtos, opt => opt.MapFrom(src => src.brandSubCategories.Select(bs => bs.Brand)));
                CreateMap<SubCategory, SubCategoryListItemInBrandDto>()
                 .ForMember(dest => dest.Image, opt => opt.MapFrom(src => url + "img/" + src.Image))
       .ForMember(dest => dest.produtListItemDtos, opt => opt.MapFrom(src => src.Products));
            CreateMap<SubCategory, SubCategoryReturnDto>()
                .ForMember(s => s.CategoryInSubcategoryReturn, map => map.MapFrom(d => d.Category))
                .ForMember(s => s.produtListItemDtos, map => map.MapFrom(d => d.Products))
                .ForMember(s => s.brandListItemDtos, map => map.MapFrom(d => d.brandSubCategories.Select(bs => bs.Brand))); // Map brands from join table
            CreateMap<SubCategoryUpdateDto, SubCategory>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (src.CategoryId != null && src.CategoryId > 0)
                    {
                        dest.CategoryId = src.CategoryId.Value;
                    }
                })
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Brand mappings
            CreateMap<Brand, BrandReturnDto>()
                .ForMember(s => s.ImageUrl, map => map.MapFrom(d => url + "img/" + d.ImageUrl))
                .ForMember(s => s.produtListItemDtos, map => map.MapFrom(d => d.Products))
                .ForMember(s => s.SubCategoryListItemInBrandDtos, map => map.MapFrom(d => d.brandSubCategories.Select(bs => bs.SubCategory))); // Map subcategories from join table
            CreateMap<BrandCreateDto, Brand>()
                .ForMember(s => s.ImageUrl, map => map.MapFrom(d => d.formFile.Save(Directory.GetCurrentDirectory(), "img")));
                CreateMap<Brand, BrandListItemDto>()
         .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => url + "img/" + src.ImageUrl))
         .ForMember(dest => dest.produtListItemDtos, opt => opt.MapFrom(src => src.Products))
                .ForMember(dest => dest.SubCategoryListItemInBrandDtos, opt => opt.MapFrom(d => d.brandSubCategories.Select(bs => bs.SubCategory)));
                CreateMap<BrandUpdateDto, Brand>()
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

            // Product mappings
            CreateMap<Product, ProdutListItemDto>()
                .ForMember(s => s.Category, map => map.MapFrom(d => d.Category));

            // Setting mappings
            CreateMap<Setting, SettingDto>().ReverseMap();
            CreateMap<SettingUpdateDto, Setting>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
                CreateMap<AddressCreateDto, Address>();
                CreateMap<Address, AddressListItemDto>()
     .ForMember(s => s.AppUser, map => map.MapFrom(d => d.appUser))
     .ForMember(s => s.AddressType, map => map.MapFrom(d => d.AddressType.ToString()));
                CreateMap<AddressUpdateDto, Address>()
               .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
                CreateMap<Address, AddressReturnDto>()
                .ForMember(s => s.AddressType, map => map.MapFrom(d => d.AddressType.ToString()));
                CreateMap<ColorCreateDto, Color>();
                CreateMap<Color, ColorListItemDto>();
            });
            configuration.AssertConfigurationIsValid();
        }
    }
}
