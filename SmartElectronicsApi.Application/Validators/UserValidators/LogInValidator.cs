using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.UserValidators
{
    public class LogInValidator:AbstractValidator<LoginDto>
    {
        public LogInValidator()
        {
            RuleFor(s => s.UserNameOrGmail).NotEmpty().WithMessage("not empty")
                .MaximumLength(200).WithMessage("max is 200");
            RuleFor(s => s.Password).NotEmpty().WithMessage("not empty")
               .MinimumLength(8)
               .MaximumLength(100).WithMessage("max is 100");
        }
    }
}
