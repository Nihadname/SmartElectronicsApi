﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.ParametrGroup;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrGroupController : ControllerBase
    {
        private readonly IParametrGroupService _parametrGroupService;

        public ParametrGroupController(IParametrGroupService parametrGroupService)
        {
            _parametrGroupService = parametrGroupService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ParametrGroupCreateDto parametrGroupCreateDto)
        {
            try
            {
                return Ok( await _parametrGroupService.Create(parametrGroupCreateDto));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex);
            }
        }
    }
}