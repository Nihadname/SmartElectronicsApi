using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using SmartElectronicsApi.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Azure.Core;
using SmartElectronicsApi.Application.Helpers;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Api.Apps.UserInterface.Dtos.Auth;
using AutoMapper;
using SmartElectronicsApi.Application.Dtos.Auth;
using Microsoft.AspNetCore.Authentication;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace SmartElectronicsApi.Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper,  IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            //_urlHelper = urlHelperFactory.GetUrlHelper(new ActionContext
            //{
            //    HttpContext = httpContextAccessor.HttpContext,
            //    RouteData = httpContextAccessor.HttpContext.GetRouteData(),
            //    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            //});
            _contextAccessor = contextAccessor;
        }

        public async Task<AppUser> FindOrCreateUserAsync(string email, string userName, string googleId)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser();
                user.GoogleId = googleId;
                user.Email = email;
                user.UserName = userName;

              
                var result=await _userManager.CreateAsync(user);
             
                if (!result.Succeeded)
                {
                    throw new CustomException(400,"There is an issue with Register ");
                }
                
                await _userManager.AddToRoleAsync(user, nameof(RolesEnum.Member));
               
            }
            return user;

        }

        public async string Login(LoginDto loginDto)
        {
            var User = await _userManager.FindByEmailAsync(loginDto.UserNameOrGmail);
            if (User == null)
            {
                User = await _userManager.FindByNameAsync(loginDto.UserNameOrGmail);
                if (User == null)
                {
                    throw new CustomException(400, "UserNameOrGmail", "userName or email is wrong\"");
                }
            }
            var result= await _userManager.CheckPasswordAsync(User, loginDto.Password);
            if (!result)
            {
                throw new CustomException(400, "Password", "userName or email is wrong\"");
            }

            throw new NotImplementedException();
        }

        public async Task<UserGetDto> Register(RegisterDto registerDto)
        {
            var existUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existUser != null) throw new CustomException(400, "UserName", "UserName is already Taken");
             var existUserEmail= await _userManager.FindByEmailAsync(registerDto.Email);
            if (existUserEmail != null) throw new CustomException(400, "Email", "Email is already taken");
            AppUser appUser = new AppUser();
            appUser.UserName = registerDto.UserName;
            appUser.Email = registerDto.Email;
            appUser.fullName = registerDto.FullName;
            appUser.GoogleId = null;
            appUser.Image=null;
            try
            {
                var result = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (!result.Succeeded) throw new CustomException(400, result.Errors.ToString());

                var MappedUser=_mapper.Map<UserGetDto>(appUser);
                return MappedUser; 
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerExceptionMessage = ex.InnerException?.Message;
                throw new CustomException(500, $"error Bas Verib {errorMessage}, Inner Exception occured : {innerExceptionMessage}");
            }

        }

        

        public async Task<GoogleGetDto> GoogleResponse()
        {
            var result = await _contextAccessor.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded) throw new CustomException(400, "Authentication failed!");
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userName = claims?.FirstOrDefault(s => s.Type == ClaimTypes.Name)?.Value;
            var Id = claims?.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            var GivenName = claims?.FirstOrDefault(s => s.Type == ClaimTypes.GivenName)?.Value;
            GoogleGetDto googleGetDto = new GoogleGetDto();
            googleGetDto.userName = userName;
            googleGetDto.GivenName = GivenName;
            googleGetDto.Email = email;
            googleGetDto.Id = Id;
            return googleGetDto;    
        }
    }
}
