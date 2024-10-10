using AutoMapper;
using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Dtos.ProductVariation;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using SmartElectronicsApi.Application.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class ProductVariationService:IProductVariationService
    {
        private readonly  IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductVariationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var ProductVariation = await _unitOfWork.productVariationRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (ProductVariation is null) throw new CustomException(404, "Not found");
            await _unitOfWork.productVariationRepository.Delete(ProductVariation);
            _unitOfWork.Commit();
            return ProductVariation.Id;
        }
        public async Task<ProductVariationCreateDto> Create(ProductVariationCreateDto productVariationCreateDto)
        {
            if (await _unitOfWork.productVariationRepository.isExists(s => s.VariationName.ToLower() == productVariationCreateDto.VariationName.ToLower()))
            {
                throw new CustomException(400, "VariationName", "This product name already exists.");
            }

            var product = await _unitOfWork.productRepository.GetEntity(s => s.Id == productVariationCreateDto.ProductId);
            if (product is null) throw new CustomException(400, "Product", "Invalid Product selected.");

            if (productVariationCreateDto.DiscountPercentage.HasValue)
            {
                productVariationCreateDto.DiscountedPrice = productVariationCreateDto.Price - (productVariationCreateDto.Price * productVariationCreateDto.DiscountPercentage.Value) / 100;
            }

            productVariationCreateDto.SKU = productVariationCreateDto.VariationName.Length >= 5
                ? productVariationCreateDto.VariationName.Substring(0, 5) + Guid.NewGuid().ToString().Substring(0, 15)
                : productVariationCreateDto.VariationName + Guid.NewGuid().ToString().Substring(0, 15);

            foreach (var colorId in productVariationCreateDto.ColorIds)
            {
                if (!await _unitOfWork.colorRepository.isExists(s => s.Id == colorId))
                {
                    throw new CustomException(400, "Color", "Invalid color selected.");
                }
            }

            var mappedProduct = _mapper.Map<ProductVariation>(productVariationCreateDto);

            await _unitOfWork.productVariationRepository.Create(mappedProduct);
             _unitOfWork.Commit();

            foreach (var colorId in productVariationCreateDto.ColorIds)
            {
                var productColor = new ProductVariationColor
                {
                    ProductVariationId = mappedProduct.Id,
                    ColorId = colorId
                };
                await _unitOfWork.ProductVariationColorRepository.Create(productColor);
            }

             _unitOfWork.Commit();

            bool isFirstImage = true;
            foreach (var image in productVariationCreateDto.Images)
            {
                var imagePath = image.Save(Directory.GetCurrentDirectory(), "img");
                var productImage = new ProductImage
                {
                    Name = Path.GetFileName(imagePath),
                    IsMain = isFirstImage,
                    ProductId = mappedProduct.Id,
                };
                await _unitOfWork.ProductImageRepository.Create(productImage);

                isFirstImage = false;
            }

           _unitOfWork.Commit();

            return productVariationCreateDto;
        }
    }
}
