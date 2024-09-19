using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.UserValidators
{
    public class ResetPasswordEmailDtoValidator:AbstractValidator<ResetPasswordEmailDto>
    {
        public ResetPasswordEmailDtoValidator()
        {
            RuleFor(s => s.Email).NotEmpty().WithMessage("not empty")
             .MinimumLength(8);
            RuleFor(s => s.Token).Empty();
        }
    }
}
