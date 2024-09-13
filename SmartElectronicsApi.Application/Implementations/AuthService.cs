using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using SmartElectronicsApi.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Azure.Core;

namespace SmartElectronicsApi.Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork unitOfWork;
        private UserManager<AppUser> userManager;
        public AuthService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<AppUser> FindOrCreateUserAsync(string email, string userName, string googleId)
        {
            var user = await unitOfWork.authRepository.GetEntity(u => u.GoogleId == googleId || u.Email == email);
            if (user == null)
            {
              AppUser appUser = new AppUser();
                appUser.GoogleId = googleId;
                appUser.Email = email;
                appUser.UserName = userName;
              
                var result=await userManager.CreateAsync(appUser);
                if (result.Succeeded)
                {
                    return appUser;
                }

            }
            throw new NotImplementedException();

        }

        public string Login()
        {
            throw new NotImplementedException();
        }

        public void Register()
        {
            throw new NotImplementedException();
        }

        public string SignInWithGoogle()
        {
            throw new NotImplementedException();
        }
    }
}
