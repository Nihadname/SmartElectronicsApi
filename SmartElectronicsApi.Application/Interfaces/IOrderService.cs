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
        Task<OrderListItemDto> CreateOrderAsync(int addressId, string token);
    }
}
