using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartElectronicsApi.Application.Dtos.Role;
using AutoMapper;
using SmartElectronicsApi.Application.Exceptions;

namespace SmartElectronicsApi.Application.Implementations
{
    public class RoleService:IRoleService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;


        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<PaginatedResponse<RoleListItemDto>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var TotalCount = ( await _roleManager.Roles.ToListAsync()).Count();
            var roles=await _roleManager.Roles.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var MappedRoles=_mapper.Map<List<RoleListItemDto>>(roles);
            return new PaginatedResponse<RoleListItemDto>
            {
                Data = MappedRoles,
                TotalRecords = TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<RoleListItemDto> GetByid(string? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null"); 
            var currentRole = await _roleManager.FindByIdAsync(id);
            if (currentRole == null) throw new CustomException(404, "Not found"); 
           var MappedRole=_mapper.Map<RoleListItemDto>(currentRole);
            return MappedRole;
        }
        public async Task<string> Delete(string? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var currentRole = await _roleManager.FindByIdAsync(id);
            if (currentRole == null) throw new CustomException(404, "Not found");
            await _roleManager.DeleteAsync(currentRole);
            return currentRole.Id;
        }
       public async Task<RoleDto> Create(RoleDto roleDto)
        {
            if (!await _roleManager.RoleExistsAsync(roleDto.Name))
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = roleDto.Name });
                return roleDto;
            }
            else
            {
                throw new CustomException(404, "Name", "this already exists");
            }
        }
        public async Task<IdentityRole> Update(string? id, RoleDto roleDto)
        {
            if (id is null) throw new CustomException(400, "Id", "id can't be null");

            var currentRole = await _roleManager.FindByIdAsync(id);
            if (currentRole == null) throw new CustomException(404, "Not found");

            if (!string.Equals(currentRole.Name, roleDto.Name, StringComparison.OrdinalIgnoreCase)
                && await _roleManager.RoleExistsAsync(roleDto.Name))
            {
                throw new CustomException(404, "This role name already exists");
            }

            _mapper.Map(roleDto, currentRole);

            IdentityResult result = await _roleManager.UpdateAsync(currentRole);
            if (result.Succeeded)
            {
                return currentRole;
            }
            else
            {
                var errorMessages = result.Errors.ToDictionary(e => e.Code, e => e.Description);
                throw new CustomException(400, errorMessages);
            }
        }
        public async Task<roleUpdateDto> GetUserRoles(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new CustomException(400, "Id", "id can't be null");
            }
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                throw new CustomException(404, "Not found");
            }
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var allRoles = await _roleManager.Roles.ToListAsync();
            var roleUpdateVm = new roleUpdateDto(userRoles, currentUser.UserName, allRoles);
            return roleUpdateVm;
        }
        public async Task<string> UpdateRole(string id, List<string> NewRoles)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new CustomException(400, "Id", "id can't be null");
            }
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                throw new CustomException(404, "Not found");
            }
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            await _userManager.RemoveFromRolesAsync(currentUser, userRoles);
            await _userManager.AddToRolesAsync(currentUser, NewRoles);
            return "Updated Roles successfully";

        }
    }
}
