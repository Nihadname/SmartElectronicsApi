﻿using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using SmartElectronicsApi.DataAccess.Migrations;
using SmartElectronicsApi.Api.Apps.UserInterface.Dtos.Auth;
using SmartElectronicsApi.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Application.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager)
        {
            _authService = authService;
            _roleManager = roleManager;
        }

        [HttpGet("SignInWithGoogle")]
        public IActionResult SignInWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleResponse)),
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var token = await _authService.GoogleResponse();

            return Redirect($"http://localhost:5041/Account/GoogleResponse?token={token}");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            return Ok(await _authService.Register(registerDto));
        }
        [HttpPost("Login")]
        
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
          return  Ok(await _authService.Login(loginDto));
        }
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
            }
          
            if (!await _roleManager.RoleExistsAsync("Member"))
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Member" });
            }
            if (!await _roleManager.RoleExistsAsync("Delivery"))
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Delivery" });
            }
            return Content("roles are added");
        }
        [HttpGet("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            bool isVerified = await _authService.VerifyEmail(email, token);
            if (isVerified)
            {
                return Redirect("https://localhost:7170/Account/Login");
            }
            else
            {
                return BadRequest(new { error = "Email verification failed" });
            }
        }
        [HttpPost("ResetPasswordSendEmail")]
        public async Task<IActionResult> ResetPasswordSendEmail(ResetPasswordEmailDto resetPasswordEmailDto)
        {
            if (!ModelState.IsValid)
            {
                throw new CustomException(StatusCodes.Status400BadRequest, "Some is empty");
            }
            var result = await _authService.ResetPasswordSendEmail(resetPasswordEmailDto);
            return Ok(new { message = result });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string token, [FromBody]ResetPasswordDto resetPasswordDto)
        {
            return Ok(await _authService.ResetPassword(email, token, resetPasswordDto));
        }
        [HttpGet("CheckExperySutiationOfToken")]
        public async Task<IActionResult> CheckExperySutiationOfToken(string email,string token)
        {
            return Ok(await _authService.CheckExperySutiationOfToken(email, token));
        }
        [HttpPost("ChangePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword( ChangePasswordDto changePasswordDto)
        {
            return Ok(await _authService.ChangePassword(changePasswordDto));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            return Ok(await _authService.Profile());
        }
        [HttpPatch("UpdateImage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> UpdateImage([FromForm] UserUpdateImageDto userUpdateImageDto)
        {
            return Ok(await _authService.UpdateImage(userUpdateImageDto));
        }
        [HttpPut("UpdateUserInformation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserInformation(UpdateUserDto updateUserDto)
        {
            return Ok(await _authService.UpdateUserInformation(updateUserDto));
        }
        [HttpGet("CheckingAuth")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public IActionResult Check()
        {
            return Ok();
        }

        [HttpGet("CheckingAuthForMember")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CheckAuthForMember()
        {
            return Ok();
        }
    }
}
