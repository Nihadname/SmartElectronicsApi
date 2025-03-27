using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartElectronicsApi.Application.Dtos.GuestOrder;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using SmartElectronicsApi.DataAccess.Migrations;

namespace SmartElectronicsApi.Application.Implementations
{
    public class GuestOrderService:IGuestOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly IEmailService _emailService;
        public GuestOrderService(IUnitOfWork unitOfWork, IMapper mapper, LinkGenerator linkGenerator, IHttpContextAccessor contextAccessor, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
            _contextAccessor = contextAccessor;
            _emailService = emailService;
        }
        public async Task<string> Create(GuestOrderCreateDto guestOrderCreateDto)
        {
          
            await ValidatProductDetails(guestOrderCreateDto);
            guestOrderCreateDto.OrderStatus = OrderStatus.Pending;

            var mappedGuestOrder = new GuestOrder
            {
                 FullName=guestOrderCreateDto.FullName,
                 Address=guestOrderCreateDto.Address,
                 Age=guestOrderCreateDto.Age,
                 EmailAdress=guestOrderCreateDto.EmailAdress,
                 ExtraInformation=guestOrderCreateDto.ExtraInformation,
                 IsGottenFromStore=guestOrderCreateDto.IsGottenFromStore,
                 OrderStatus= (OrderStatus)guestOrderCreateDto.OrderStatus,
                 PhoneNumber=guestOrderCreateDto.PhoneNumber,
                 ProductName=guestOrderCreateDto.ProductName,
                 ProductPrice= (decimal)guestOrderCreateDto.ProductPrice,
                 PurchasedProductId=guestOrderCreateDto.PurchasedProductId,
                 PurchasedProducVariationtId=guestOrderCreateDto.PurchasedProducVariationtId??null,
               
                

            };
            await _unitOfWork.GuestOrderRepository.Create(mappedGuestOrder);
            _unitOfWork.Commit();
            string link = _linkGenerator.GetUriByAction(
                httpContext: _contextAccessor.HttpContext,
                action: "VerifyGuestOrder",
            controller: "GuestOrder",
                values: new { id = mappedGuestOrder.Id},
                scheme: _contextAccessor.HttpContext.Request.Scheme,
                host: _contextAccessor.HttpContext.Request.Host
            );
            string body;
            using (StreamReader sr = new StreamReader("wwwroot/Template/emailConfirm.html"))
            {
                body = sr.ReadToEnd();
            }
            body = body.Replace("{{link}}", link).Replace("{{UserName}}", mappedGuestOrder.FullName);

            _emailService.SendEmail(
                from: "nihadmi@code.edu.az\r\n",
                to: mappedGuestOrder.EmailAdress,
                subject: "Verify Email",
                body: body,
                smtpHost: "smtp.gmail.com",
                smtpPort: 587,
                enableSsl: true,
                smtpUser: "nihadmi@code.edu.az\r\n"

            );
            return "Created GuestOrder";
        }
        public async Task<string> VerifyEmail(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var guestOrder = await _unitOfWork.GuestOrderRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (guestOrder is null) throw new CustomException(404, "Not found");
            if (guestOrder.OrderStatus != OrderStatus.Pending) throw new CustomException(400, "Order", "this order is already compeleted or in diffrent status ");
            guestOrder.OrderStatus = OrderStatus.Completed;
            _unitOfWork.Commit();
            return "it is completed";
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
