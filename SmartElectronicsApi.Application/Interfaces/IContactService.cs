using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IContactService
    {
        Task<ContactCreateDto> Create(ContactCreateDto contactCreateDto);
        Task<PaginatedResponse<ContactDto>> GetAll(int pageNumber = 1,
           int pageSize = 10);
    }
}
