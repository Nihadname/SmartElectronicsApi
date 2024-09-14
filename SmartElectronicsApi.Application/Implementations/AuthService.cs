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
using Microsoft.Extensions.Options;
using SmartElectronicsApi.Application.Settings;

namespace SmartElectronicsApi.Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly LinkGenerator _linkGenerator;

        public AuthService(IOptions<JwtSettings> jwtSettings, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IHttpContextAccessor contextAccessor, ITokenService tokenService, IEmailService emailService, SignInManager<AppUser> signInManager, LinkGenerator linkGenerator)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
            _signInManager = signInManager;
            _linkGenerator = linkGenerator;
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

        public async Task<string> Login(LoginDto loginDto)
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
            if (!User.EmailConfirmed)
            {
                throw new CustomException(400, "UserNameOrGmail", "email not confirmed");

            }
            IList<string> roles = await _userManager.GetRolesAsync(User);
            var Audience = _jwtSettings.Audience;
            var SecretKey = _jwtSettings.secretKey;
            var Issuer = _jwtSettings.Issuer;
            return _tokenService.GetToken(SecretKey, Audience, Issuer, User, roles);
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
                await _userManager.AddToRoleAsync(appUser, RolesEnum.Member.ToString());
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                string link = _linkGenerator.GetUriByAction(
                    httpContext: _contextAccessor.HttpContext,
                    action: "VerifyEmail",
                    controller: "Auth",
                    values: new { email = appUser.Email, token = token },
                    scheme: _contextAccessor.HttpContext.Request.Scheme,
                    host: _contextAccessor.HttpContext.Request.Host
                );
                string body;
                using (StreamReader sr = new StreamReader("wwwroot/Template/emailConfirm.html"))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{{link}}", link).Replace("{{UserName}}", appUser.UserName);

                _emailService.SendEmail(
                    from: "nihadmi@code.edu.az\r\n",
                    to: appUser.Email,
                    subject: "Verify Email",
                    body: body,
                    smtpHost: "smtp.gmail.com",
                    smtpPort: 587,
                    enableSsl: true,
                    smtpUser: "nihadmi@code.edu.az\r\n",
                    smtpPass: "zrhu njzc qeqr koux\r\n"
                );
                var MappedUser =_mapper.Map<UserGetDto>(appUser);
                return MappedUser; 
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerExceptionMessage = ex.InnerException?.Message;
                throw new CustomException(500, $"error Bas Verib {errorMessage}, Inner Exception occured : {innerExceptionMessage}");
            }

        }

        public async Task<string> VerifyEmail(string email, string token)
        { 
            AppUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser is null) throw new CustomException(404, "User is null");
            await _userManager.ConfirmEmailAsync(appUser, token);
            // await _signInManager.SignInAsync(appUser, true);
            return "https://github.com/Nihadname";
        }

        public async Task<string> GoogleResponse()
        {
            var result = await _contextAccessor.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded) throw new CustomException(400, "Authentication failed!");
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userName = claims?.FirstOrDefault(s => s.Type == ClaimTypes.Name)?.Value;
            var Id = claims?.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            var GivenName = claims?.FirstOrDefault(s => s.Type == ClaimTypes.GivenName)?.Value;
            var user =await FindOrCreateUserAsync(email, userName, Id);
            IList<string> roles = await _userManager.GetRolesAsync(user);
            var Audience = _jwtSettings.Audience;
            var SecretKey = _jwtSettings.secretKey;
            var Issuer = _jwtSettings.Issuer;

            //GoogleGetDto googleGetDto = new GoogleGetDto();
            //googleGetDto.userName = userName;
            //googleGetDto.GivenName = GivenName;
            //googleGetDto.Email = email;
            //googleGetDto.Id = Id;

            return _tokenService.GetToken(SecretKey, Audience, Issuer, user, roles);
        }
    }
}
