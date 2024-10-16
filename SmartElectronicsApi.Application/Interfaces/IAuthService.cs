using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Api.Apps.UserInterface.Dtos.Auth;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Core.Entities;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<string> GoogleResponse();
        public Task<string> Login(LoginDto loginDto);
        public Task<UserGetDto> Register(RegisterDto registerDto);
        Task<AppUser> FindOrCreateUserAsync(string email, string googleId, string GivenName);
        Task<bool> VerifyEmail(string email, string token);
        Task<ResetPasswordEmailDto> ResetPasswordSendEmail(ResetPasswordEmailDto resetPasswordEmailDto);     
            Task<string> ResetPassword(string email, string token, ResetPasswordDto resetPasswordDto);
        Task<string> CheckExperySutiationOfToken(string email, string token);
        Task<string> ChangePassword(ChangePasswordDto changePasswordDto);
        Task<UserGetDto> Profile();
        Task<string> UpdateImage(UserUpdateImageDto userUpdateImageDto);
        Task<string> UpdateUserInformation(UpdateUserDto updateUserDto);
        Task<PaginatedResponse<UserGetDto>> GetAll(int pageNumber = 1, int pageSize = 10);
        Task<string> Delete(string? id);
        Task<string> ChangeStatus(string id);
    }
}
