﻿namespace SmartElectronicsApi.Api.Apps.UserInterface.Dtos.Auth
{
    public class RegisterDto
    {

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }
}