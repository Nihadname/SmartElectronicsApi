using SmartElectronicsApi.Application.Dtos.GuestOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IGuestOrderService
    {
        Task<string> Create(GuestOrderCreateDto guestOrderCreateDto);
    }
}
