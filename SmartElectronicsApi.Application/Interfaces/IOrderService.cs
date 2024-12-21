using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Order;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IOrderService
    {
        Task<string> CreateStripeCheckoutSessionAsync();
        Task<string> VerifyPayment(string sessionId);
       Task<PaginatedResponse<OrderAdminListItemDto>> GetAll(int pageNumber = 1, int pageSize = 10);
        Task<PaginatedResponse<OrderListItemDto>> GetAllForUser(int pageNumber = 1, int pageSize = 10);
        Task<string> ShippingOrder(int? Id);
        Task<string> VerifyOrderAsDelivered(OrderVerifyDto orderVerifyDto);
        Task<OrderReturnDto> GetById(int? Id);
        Task<string> Delete(int? Id);
        Task<string> DeleteOrderForUser(int? Id);
    }
}
