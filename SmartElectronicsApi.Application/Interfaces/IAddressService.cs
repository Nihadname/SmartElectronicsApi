﻿using SmartElectronicsApi.Application.Dtos.Address;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IAddressService
    {
       Task<Address> Create(AddressCreateDto addressCreateDto);
        Task<int> Delete(int? id);
        Task<List<AddressListItemDto>> GetAll();


    }
}