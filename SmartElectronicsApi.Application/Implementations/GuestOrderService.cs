using AutoMapper;
using SmartElectronicsApi.Application.Dtos.GuestOrder;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;

namespace SmartElectronicsApi.Application.Implementations
{
    public class GuestOrderService:IGuestOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GuestOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<string> Create(GuestOrderCreateDto guestOrderCreateDto)
        {
           await ValidateGuestOrder(guestOrderCreateDto);
            await ValidatProductDetails(guestOrderCreateDto);
            guestOrderCreateDto.OrderStatus = OrderStatus.Completed;
            var mappedGuestOrder = _mapper.Map<GuestOrder>(guestOrderCreateDto);
            await _unitOfWork.GuestOrderRepository.Create(mappedGuestOrder);
            _unitOfWork.Commit();
            return "";
        }
        private  async Task ValidatProductDetails(GuestOrderCreateDto guestOrderCreateDto)
        {
            var product = await _unitOfWork.productRepository.GetEntity(s => s.Id == guestOrderCreateDto.PurchasedProductId)
       ?? throw new CustomException(400, "Product", "Product with this ID does not exist.");

            guestOrderCreateDto.ProductName = product.Name;
            guestOrderCreateDto.ProductPrice = product.DiscountedPrice ?? product.Price;

            if (guestOrderCreateDto.PurchasedProducVariationtId is not null&&guestOrderCreateDto.PurchasedProducVariationtId!=0)
            {
                var productVariation = await _unitOfWork.productVariationRepository.GetEntity(s =>
                    s.ProductId == guestOrderCreateDto.PurchasedProductId &&
                    s.Id == guestOrderCreateDto.PurchasedProducVariationtId)
                    ?? throw new CustomException(400, "ProductVariation", "Product variation with this ID does not exist.");

                guestOrderCreateDto.ProductName = productVariation.VariationName;
                guestOrderCreateDto.ProductPrice = productVariation.DiscountedPrice ?? productVariation.Price;
            }
        }
        private async Task ValidateGuestOrder(GuestOrderCreateDto guestOrderCreateDto)
        {
            bool isDuplicate = await _unitOfWork.GuestOrderRepository.isExists(s =>
                   s.FullName.ToLower() == guestOrderCreateDto.FullName.ToLower() ||
                 s.PhoneNumber.ToLower() == guestOrderCreateDto.PhoneNumber.ToLower() ||
                   s.EmailAdress.ToLower() == guestOrderCreateDto.EmailAdress.ToLower()
                      );
            //bool isNameRepeated = await _unitOfWork.GuestOrderRepository.isExists(s => s.FullName.ToLower() == guestOrderCreateDto.FullName.ToLower());
            //bool isPhoneNumberRepeated = await _unitOfWork.GuestOrderRepository.isExists(s => s.PhoneNumber.ToLower() == guestOrderCreateDto.PhoneNumber.ToLower());
            //bool isEmailAdressRepeated = await _unitOfWork.GuestOrderRepository.isExists(s => s.EmailAdress.ToLower() == guestOrderCreateDto.EmailAdress.ToLower());

            if (isDuplicate)
                throw new CustomException(400, "GuestOrder", "user with this Name,phoneNumber,Email has  alrady done a purchase ");
            
        }
    }
}
