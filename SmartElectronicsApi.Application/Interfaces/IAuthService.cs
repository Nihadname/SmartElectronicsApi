using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Api.Apps.UserInterface.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Core.Entities;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<GoogleGetDto> GoogleResponse();
        public string Login();
        public Task<UserGetDto> Register(RegisterDto registerDto);
        Task<AppUser> FindOrCreateUserAsync(string email, string userName, string googleId);

    }
}
