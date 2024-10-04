using Microsoft.AspNetCore.Identity;
using SmartElectronicsApi.Application.Dtos.Role;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IRoleService
    {
        Task<PaginatedResponse<RoleListItemDto>> GetAll(int pageNumber = 1, int pageSize = 10);
        Task<RoleListItemDto> GetByid(string? id);
        Task<string> Delete(string? id);
        Task<RoleDto> Create(RoleDto roleDto);
        Task<IdentityRole> Update(string? id, RoleDto roleDto);
    }
}
