using AutoMapper;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Color;
using SmartElectronicsApi.Application.Dtos.Subscriber;
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
    public class SubscriberService:ISubscriberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SubscriberService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<SubscriberDto> Create(SubscriberDto subscriberDto)
        {
            if (await _unitOfWork.subscriberRepository.isExists(s => s.Email.ToLower() == subscriberDto.Email.ToLower()))
            {
                throw new CustomException(400, "Code", "your email already exist in our system");
            }
            var MappedOne = _mapper.Map<Subscriber>(subscriberDto);
            await _unitOfWork.subscriberRepository.Create(MappedOne);
            _unitOfWork.Commit();
            return subscriberDto;

        }
        public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var subscriber = await _unitOfWork.subscriberRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (subscriber is null) throw new CustomException(404, "Not found");
            await _unitOfWork.subscriberRepository.Delete(subscriber);
            _unitOfWork.Commit();
            return subscriber.Id;
        }
        public async Task<PaginatedResponse<SubscriberDto>> GetAllForAdminUi(int pageNumber = 1, int pageSize = 10)
        {
            var totalCount = (await _unitOfWork.subscriberRepository.GetAll()).Count();
            var subscribers = await _unitOfWork.subscriberRepository.GetAll(s => s.IsDeleted == false,
                                                                  (pageNumber - 1) * pageSize,
                                                                  pageSize);
         var   subscribersMapping=_mapper.Map<List<SubscriberDto>>(subscribers);
            return new PaginatedResponse<SubscriberDto>
            {
                Data = subscribersMapping,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<Subscriber> Update(int? id, SubscriberDto subscriberDto)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var subscriber = await _unitOfWork.subscriberRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (subscriber is null) throw new CustomException(404, "Not found");
            if (subscriberDto.Email != null)
            {
                if (await _unitOfWork.subscriberRepository.isExists(s => s.Email.ToLower() == subscriberDto.Email.ToLower()))
                {
                    throw new CustomException(400, "Code", "your email already exist in our system");
                }

            }
            subscriber.Email=subscriberDto.Email??subscriber.Email;
            await _unitOfWork.subscriberRepository.Update(subscriber);
            _unitOfWork.Commit();
            return subscriber;
        }
    }
}
