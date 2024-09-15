using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.UserValidators
{
    public class ResetPasswordValidator:AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(s => s.Password).NotEmpty().WithMessage("not empty")
                .MinimumLength(8)
                .MaximumLength(100).WithMessage("max is 100");
            RuleFor(s => s.RePassword).NotEmpty().WithMessage("not empty")
                                .MinimumLength(8)
           .MaximumLength(100).WithMessage("max is 100");
            RuleFor(s => s.Password).Equal(s => s.RePassword);

        }
    }
}
