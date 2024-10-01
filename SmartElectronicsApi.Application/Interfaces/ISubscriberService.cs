using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Subscriber;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ISubscriberService
    {
        Task<SubscriberDto> Create(SubscriberDto subscriberDto);
        Task<int> Delete(int? id);
        Task<PaginatedResponse<SubscriberDto>> GetAllForAdminUi(int pageNumber = 1, int pageSize = 10);
        Task<Subscriber> Update(int? id, SubscriberDto subscriberDto);
    }
}
