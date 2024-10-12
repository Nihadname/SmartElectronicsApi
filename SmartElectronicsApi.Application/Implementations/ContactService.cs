using AutoMapper;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Color;
using SmartElectronicsApi.Application.Dtos.Contact;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class ContactService:IContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ContactService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PaginatedResponse<ContactDto>> GetAll(int pageNumber = 1,
           int pageSize = 10)
        {
            var totalCount = (await _unitOfWork.colorRepository.GetAll()).Count();
            var contacts = await _unitOfWork.ContactRepository.GetAll(s => s.IsDeleted == false,
                                                                  (pageNumber - 1) * pageSize,
                                                                  pageSize);
            var contactsWithMapping = _mapper.Map<List<ContactDto>>(contacts);
            return new PaginatedResponse<ContactDto>
            {
                Data = contactsWithMapping,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var contact = await _unitOfWork.ContactRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (contact is null) throw new CustomException(404, "Not found");
            await _unitOfWork.ContactRepository.Delete(contact);
            _unitOfWork.Commit();
            return contact.Id;
        } 
        public async Task<ContactCreateDto> Create(ContactCreateDto contactCreateDto)
        {
           var MappedContact=_mapper.Map<Contact>(contactCreateDto);
            await _unitOfWork.ContactRepository.Create(MappedContact);
            _unitOfWork.Commit();
            return contactCreateDto;
        }
    }
}
