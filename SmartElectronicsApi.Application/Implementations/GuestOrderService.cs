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
            bool isNameRepeated = await _unitOfWork.GuestOrderRepository.isExists(s => s.FullName.Equals(guestOrderCreateDto.Name, StringComparison.OrdinalIgnoreCase));
            var isPhoneNumberRepeated = await _unitOfWork.GuestOrderRepository.isExists(s => s.PhoneNumber.Equals(guestOrderCreateDto.PhoneNumber, StringComparison.OrdinalIgnoreCase));
            var isEmailAdressRepeated=await _unitOfWork.GuestOrderRepository.isExists(s=>s.EmailAdress.Equals(guestOrderCreateDto.EmailAdress, StringComparison.OrdinalIgnoreCase));
            if (isNameRepeated||isPhoneNumberRepeated||isEmailAdressRepeated)
               throw new CustomException(400,"GuestOrder", "user with this Name,phoneNumber,Email has  alrady done a purchase ");
            var isProductIdValid=await _unitOfWork.productRepository.isExists(s=>s.Id==guestOrderCreateDto.PurchasedProductId);
            var isProductVariationId = await _unitOfWork.productVariationRepository.isExists(s => s.Id == guestOrderCreateDto.PurchasedProductId);
            if(!isProductIdValid&&!isProductVariationId) throw new CustomException(400, "Product", "Product with this id doesnt exist");
            var mappedGuestOrder = _mapper.Map<GuestOrder>(guestOrderCreateDto);
            mappedGuestOrder.OrderStatus = OrderStatus.Completed;

            return "";
        }
    }
}
