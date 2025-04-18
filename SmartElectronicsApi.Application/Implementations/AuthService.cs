﻿using SmartElectronicsApi.Application.Interfaces;
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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using SmartElectronicsApi.Application.Extensions;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.DataAccess.Data;


namespace SmartElectronicsApi.Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
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
        private readonly SmartElectronicsDbContext _smartElectronicsDbContext;
        public AuthService(IOptions<JwtSettings> jwtSettings, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IHttpContextAccessor contextAccessor, ITokenService tokenService, IEmailService emailService, SignInManager<AppUser> signInManager, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor, SmartElectronicsDbContext smartElectronicsDbContext)
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
            _httpContextAccessor = httpContextAccessor;
            _smartElectronicsDbContext = smartElectronicsDbContext;
        }

        public async Task<AppUser> FindOrCreateUserAsync(string email, string googleId, string GivenName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser();
                user.GoogleId = googleId;
                user.Email = email;
                user.UserName = email.Split('@')[0];
                user.fullName = GivenName;
                user.Image = null;
                user.EmailConfirmed = true;
                user.CreatedTime= DateTime.UtcNow;
                user.loyalPoints = 0;
                user.loyaltyTier = 1;
                var result = await _userManager.CreateAsync(user, "User_@" + Guid.NewGuid().ToString().Substring(0, 14));

                if (!result.Succeeded)
                {
                    var errorMessages = result.Errors.ToDictionary(e => e.Code, e => e.Description);

                    throw new CustomException(400, errorMessages);
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
            var result = await _userManager.CheckPasswordAsync(User, loginDto.Password);

            if (!result)
            {
                throw new CustomException(400, "Password", "Password or email is wrong\"");
            }
            if (!User.EmailConfirmed)
            {
                throw new CustomException(400, "UserNameOrGmail", "email not confirmed");

            }
            if (User.IsBlocked && User.BlockedUntil.HasValue)
            {
                if (User.BlockedUntil.Value <= DateTime.UtcNow)
                {
                    User.IsBlocked = false;
                    User.BlockedUntil = null;
                    await _userManager.UpdateAsync(User);
                }
                else
                {
                    throw new CustomException(403, "UserNameOrGmail", $"you are blocked until {User.BlockedUntil?.ToString("dd MMM yyyy hh:mm")}");
                }
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
            var existUserEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existUserEmail != null) throw new CustomException(400, "Email", "Email is already taken");
            AppUser appUser = new AppUser();
            appUser.UserName = registerDto.UserName;
            appUser.Email = registerDto.Email;
            appUser.fullName = registerDto.FullName;
            appUser.PhoneNumber = registerDto.PhoneNumber;
            appUser.GoogleId = null;
            appUser.Image = null;
            appUser.loyalPoints = 0;
            appUser.CreatedTime = DateTime.UtcNow;
            appUser.loyaltyTier = 1;
            var result = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!result.Succeeded)
            {
                var errorMessages = result.Errors.ToDictionary(e => e.Code, e => e.Description);

                throw new CustomException(400, errorMessages);
            }
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
                smtpUser: "nihadmi@code.edu.az\r\n"
                
            );
            var MappedUser = _mapper.Map<UserGetDto>(appUser);
            return MappedUser;



        }

        public async Task<bool> VerifyEmail(string email, string token)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser is null) throw new CustomException(404, "User is null");
            var result = await _userManager.ConfirmEmailAsync(appUser, token);
            // await _signInManager.SignInAsync(appUser, true);
            return result.Succeeded;
        }

        public async Task<string> GoogleResponse()
        {
            var result = await _contextAccessor.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null) throw new CustomException(400, "Authentication failed!");
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userName = claims?.FirstOrDefault(s => s.Type == ClaimTypes.Name)?.Value;
            var Id = claims?.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            var GivenName = claims?.FirstOrDefault(s => s.Type == ClaimTypes.GivenName)?.Value;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(Id))
            {
                throw new CustomException(400, "Failed to retrieve user information from Google.");
            }
            var user = await FindOrCreateUserAsync(email, Id, GivenName);
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

        public async Task<ResetPasswordEmailDto> ResetPasswordSendEmail(ResetPasswordEmailDto resetPasswordEmailDto)
        {
            if (string.IsNullOrEmpty(resetPasswordEmailDto.Email))
            {
                throw new CustomException(400, "Email is required.");
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordEmailDto.Email);
            if (user == null)
            {
                throw new CustomException(404, "User not found.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            resetPasswordEmailDto.Token = token;



            return resetPasswordEmailDto;
        }
        public async Task<string> ResetPassword(string email, string token, ResetPasswordDto resetPasswordDto)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new CustomException(400, "Email is required.");
            }
            if (string.IsNullOrEmpty(token))
            {
                throw new CustomException(400, "token is required.");
            }


            await CheckExperySutiationOfToken(email, token);
            var existedUser = await _userManager.FindByEmailAsync(email);
            if (existedUser == null) throw new CustomException(404, "User is null or empty");
            var result = await _userManager.ResetPasswordAsync(existedUser, token, resetPasswordDto.Password);
            if (!result.Succeeded) throw new CustomException(400, result.Errors.ToString());
            await _userManager.UpdateSecurityStampAsync(existedUser);
            return "password Reseted";
        }
        public async Task<string> CheckExperySutiationOfToken(string email, string token)
        {
            if (string.IsNullOrEmpty(email))
                throw new CustomException(400, "Email is required.");
            if (string.IsNullOrEmpty(token))
                throw new CustomException(400, "Token is required.");

            var existUser = await _userManager.FindByEmailAsync(email);
            if (existUser == null) throw new CustomException(404, "User is null or empty");
            bool result = await _userManager.VerifyUserTokenAsync(
    existUser,
    _userManager.Options.Tokens.PasswordResetTokenProvider,
    "ResetPassword",
    token
);

            if (!result)
                throw new CustomException(400, "The token is either invalid or has expired.");
            return "hasnt still expired";


        }
        public async Task<string> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
            {
                throw new CustomException(400, "Id", "User  cannot be null");

            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded) {
                var errorMessages = result.Errors.ToDictionary(e => e.Code, e => e.Description);
                throw new CustomException(400, errorMessages);
            }
            return result.ToString();
        }
        public async Task<UserGetDto> Profile()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "UserId", "User ID cannot be null or empty");
            }

            var user = await _smartElectronicsDbContext.Users
                            .Include(s => s.orders)
                            .ThenInclude(s=>s.Items)
                            .ThenInclude(s=>s.Product).ThenInclude(s=>s.Category)
                            .Include(s=>s.wishList)
                            .ThenInclude(s=>s.wishListProducts)// Ensure Orders is correctly named and virtual in your entity
                            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new CustomException(403, "UserNotFound", "This user does not exist");
            }

            var userMapped = _mapper.Map<UserGetDto>(user);
            return userMapped;
        }


        public async Task<string> UpdateImage(UserUpdateImageDto userUpdateImageDto)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new CustomException(403, "this user doesnt exist");
            if (!string.IsNullOrEmpty(user.Image))
            {
                user.Image.DeleteFile();
            }
            user.Image = userUpdateImageDto.formFile.Save(Directory.GetCurrentDirectory(), "img");
            await _userManager.UpdateAsync(user);
            return user.Image;

        }
        public async Task<string> UpdateUserInformation(UpdateUserDto updateUserDto)
        {

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");

            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new CustomException(403, "this user doesnt exist");
            _mapper.Map(updateUserDto, user);
            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                user.EmailConfirmed = false;
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string link = _linkGenerator.GetUriByAction(
                    httpContext: _contextAccessor.HttpContext,
                    action: "VerifyEmail",
                controller: "Auth",
                    values: new { email = updateUserDto.Email, token = token },
                    scheme: _contextAccessor.HttpContext.Request.Scheme,
                    host: _contextAccessor.HttpContext.Request.Host
                );
                string body;
                using (StreamReader sr = new StreamReader("wwwroot/Template/emailConfirm.html"))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{{link}}", link).Replace("{{UserName}}", user.UserName);

                _emailService.SendEmail(
                    from: "nihadmi@code.edu.az\r\n",
                    to: updateUserDto.Email,
                    subject: "Verify Email",
                    body: body,
                    smtpHost: "smtp.gmail.com",
                    smtpPort: 587,
                    enableSsl: true,
                    smtpUser: "nihadmi@code.edu.az\r\n"
                    
                );
            }
            await _userManager.UpdateAsync(user);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new CustomException(400, "Update Failed", errors);
            }
            return user.UserName;


        }
        public async Task<PaginatedResponse<UserGetDto>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var TotalCount = _userManager.Users.Count();
            var users=await _userManager.Users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var MappedUsers=_mapper.Map<List<UserGetDto>>(users);
            return new PaginatedResponse<UserGetDto>
            {
                Data = MappedUsers,
                TotalRecords = TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<string> Delete(string? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) throw new CustomException(404, "Not found");
            await _userManager.DeleteAsync(user);
            return user.Id;
        }
        public async Task<string> ChangeStatus(string id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser is null) throw new CustomException(404, "Not found");
            if (!currentUser.IsBlocked)
            {
                currentUser.IsBlocked = true;
                currentUser.BlockedUntil = DateTime.UtcNow.AddMinutes(60);
            }
            else
            {
                currentUser.IsBlocked = false;
                currentUser.BlockedUntil = null;
            }

            await _userManager.UpdateAsync(currentUser);
        return currentUser.Id;
        }
    }
}
