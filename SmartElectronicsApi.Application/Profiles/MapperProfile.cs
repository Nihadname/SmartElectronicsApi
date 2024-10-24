using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SmartElectronicsApi.Api.Apps.UserInterface.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos.Address;
using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos.Basket;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Dtos.Color;
using SmartElectronicsApi.Application.Dtos.Comment;
using SmartElectronicsApi.Application.Dtos.Contact;
using SmartElectronicsApi.Application.Dtos.Order;
using SmartElectronicsApi.Application.Dtos.ParametrGroup;
using SmartElectronicsApi.Application.Dtos.ParametrValue;
using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Dtos.ProductVariation;
using SmartElectronicsApi.Application.Dtos.Role;
using SmartElectronicsApi.Application.Dtos.Setting;
using SmartElectronicsApi.Application.Dtos.Slider;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
using SmartElectronicsApi.Application.Dtos.Subscriber;
using SmartElectronicsApi.Application.Dtos.WishList;
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
                      .ForMember(s => s.Image, map => map.MapFrom(d => url + "img/" + d.Image))
                           .ForMember(s => s.orders, map => map.MapFrom(d => d.orders.Take(5).ToList()))
                            .ForMember(s => s.WishListedItemsCount, map => map.MapFrom(d => d.wishList.wishListProducts.Count()))
                 .ForMember(s => s.TotalAmountSum, map => map.MapFrom(d => d.orders.Sum(s=>s.TotalAmount)))
                .ForMember(s => s.FavoriteCategories, map => map.MapFrom(d =>
        d.orders.SelectMany(o => o.Items)
                .GroupBy(i => i.Product.Category.Name) // Group by category name
                .OrderByDescending(g => g.Count())    // Order by most bought
                .Take(3)                              // Take top 5 categories
                .Select(g => g.Key).ToList()));
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
               .ForMember(dest => dest.ProdutListItemDtos, opt => opt.MapFrom(src => src.Products));
            CreateMap<CategoryUpdateDto, Category>()
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Category, CategoryReturnDto>()
                .ForMember(s => s.Immage, map => map.MapFrom(d => url + "img/" + d.ImageUrl))
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
                .ForMember(dest => dest.produtListItemDtos, opt => opt.MapFrom(src => src.Products));
            CreateMap<Category, CategoryInSubcategoryReturnDto>();
                CreateMap<Category, CategoryInProductListItemDto>();
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
                .ForMember(s => s.BrandIds, map => map.MapFrom(d => d.brandSubCategories.Select(s=>s.Brand.Id)))
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
                CreateMap<Setting, SettingReturnDto>();
            // Product mappings
            CreateMap<Product, ProdutListItemDto>()
                .ForMember(s => s.Category, map => map.MapFrom(d => d.Category))
    .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.productImages.Select(pi => url + "img/" + pi.Name))) 
                    .ForMember(dest => dest.colorListItemDtos, opt => opt.MapFrom(src => src.productColors.Select(s=>s.Color)))
                     .ForMember(s => s.parametrGroupListItemDtos, map => map.MapFrom(d => d.parametricGroups));

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
                CreateMap<Subscriber, SubscriberDto>().ReverseMap();
                CreateMap<ColorUpdateDto, Color>()
                 .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
                CreateMap<IdentityRole, RoleListItemDto>();
                CreateMap<RoleDto, IdentityRole>();
                CreateMap<ProductCreateDto, Product>();
                CreateMap<ProductVariationCreateDto, ProductVariation>();
                CreateMap<ProductVariation, ProductVariationListItemDto>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.productImages.Select(pi => url + "img/" + pi.Name)))
                    .ForMember(dest => dest.colorListItemDtos, opt => opt.MapFrom(src => src.productVariationColors.Select(s => s.Color)));
                //.ForMember(s => s.parametricGroups, map => map.MapFrom(d => d.ParametrGroupCreateDtos));
                // .ForMember(dest => dest.productColors, opt => opt.MapFrom(src => src.ColorIds.Select(cid => new ProductColor { ColorId = cid })));


                CreateMap<Product, ProductReturnDto>()
                .ForMember(s => s.Category, map => map.MapFrom(d => d.Category))
    .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.productImages.Select(pi => url + "img/" + pi.Name)))
                    .ForMember(dest => dest.colorListItemDtos, opt => opt.MapFrom(src => src.productColors.Select(s => s.Color)))
                     .ForMember(s => s.parametrGroupListItemDtos, map => map.MapFrom(d => d.parametricGroups))
                  .ForMember(s => s.productVariationListItemDtos, map => map.MapFrom(d => d.Variations));
                CreateMap<ParametrGroupCreateDto, ParametrGroup>()
     .ForMember(s => s.parametrValues, map => map.MapFrom(d => d.parametrValues));

                CreateMap<ParametrValueListItemDto, ParametrValue>();
                CreateMap<ParametrValue, ParametrValueListItemDto>();
                CreateMap<ParametrGroup,ParametrGroupListItemDto>();
                CreateMap<Comment, CommentListItemDto>()
        .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.commentImages.Select(pi => url + "img/" + pi.Name)))
        .ForMember(dest => dest.AppUser, opt => opt.MapFrom(src => src.AppUser));
                CreateMap<ContactCreateDto, Contact>();
                CreateMap<Contact,ContactDto>();
                CreateMap<Basket, UserBasketDto>()
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.AppUserId))
                
               .ForMember(dest => dest.BasketProducts, opt => opt.MapFrom(src => src.BasketProducts))
               .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.BasketProducts.Count()));
                CreateMap<BasketProduct, BasketListItemDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                 .ForMember(dest => dest.VariationId, opt => opt.MapFrom(src => src.ProductVariationId))
    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ProductVariation != null
        ? url + "img/" + src.ProductVariation.productImages.FirstOrDefault().Name // Use variation image if exists
        : url + "img/" + src.Product.productImages.FirstOrDefault().Name)) // Fall back to product image
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductVariation != null
        ?   src.ProductVariation.VariationName  // Use variation name if exists
        : src.Product.Name)) // Fall back to product name
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name)) // Map category name
    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductVariation != null
        ? src.ProductVariation.Price // Use variation price if exists
        : src.Product.Price)) // Fall back to product price
    .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src => src.ProductVariation != null
        ? src.ProductVariation.DiscountedPrice ?? src.ProductVariation.Price // Use variation discounted price if exists
        : src.Product.DiscountedPrice ?? src.Product.Price)) // Fall back to product discounted price
    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));// Map quantity
                                                                               // Leave null if no variation
                CreateMap<WishList, UserWishListDto>()
                               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.AppUserId))
                                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.wishListProducts.Count()));
                CreateMap<WishListProduct, WishListProductListItemDto>()
     .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
     .ForMember(dest => dest.Image, opt => opt.MapFrom(src => url + "img/" + src.Product.productImages.FirstOrDefault().Name))
     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
     .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
         src.Product.Description.Length > 60
         ? src.Product.Description.Substring(0, 50) + "..."
         : src.Product.Description
     ))
          .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Product.CreatedTime));

                CreateMap<Subscriber, SubscriberListItemDto>();

                CreateMap<Order, OrderListItemDto>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.GetName(typeof(OrderStatus), src.Status)))

     .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new OrderItemSummaryDto
     {
         ProductId = i.ProductId,
         ProductName = i.Product.Name,
         Quantity = i.Quantity,
         UnitPrice = (decimal)((i.ProductVariation != null && i.ProductVariation.DiscountedPrice > 0)
             ? i.ProductVariation.DiscountedPrice // Use variation's discounted price if available
             : (i.Product.DiscountedPrice > 0 ? i.Product.DiscountedPrice : i.UnitPrice)), // Fallback to product's discounted price or normal price
         ProductVariationId = i.ProductVariationId,
     }).ToList()));

                CreateMap<CommentCreateDto, Comment>();
                CreateMap<CommentImageDto, CommentImage>();
                //.ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Product.Price * src.Quantity))
                CreateMap<ProductUpdateDto, Product>()
          .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
                //.ForMember(dest => dest.TotalSalePrice, opt => opt.MapFrom(src => (src.Product.DiscountedPrice ?? src.Product.Price) * src.Quantity))

                //.ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Product.Price - (src.Product.DiscountedPrice ?? src.Product.Price)));
            });
            configuration.AssertConfigurationIsValid();
        }
    }
}
