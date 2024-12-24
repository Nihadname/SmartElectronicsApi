using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.DataAccess.Data.Implementations;

namespace SmartElectronicsApi.Application.Implementations
{
    public class GuestOrderService:IGuestOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GuestOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

    }
}
