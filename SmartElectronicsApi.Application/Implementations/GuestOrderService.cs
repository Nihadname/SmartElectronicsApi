using AutoMapper;
using SmartElectronicsApi.Application.Dtos.GuestOrder;
using SmartElectronicsApi.Application.Interfaces;
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
        public async Task<string> Create(GuestOrderCreateDto guestOrderCreateDto )
        {
            return "";
        }
    }
}
