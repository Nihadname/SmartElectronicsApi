﻿using AutoMapper;
using SmartElectronicsApi.Application.Dtos.ParametrGroup;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IParametrGroupService 
    {
        Task<int> Create(ParametrGroupCreateDto parametrGroupCreateDto);
       
    }
}