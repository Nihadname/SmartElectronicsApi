using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.UserValidators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(s => s.CurrentPassword).NotEmpty().WithMessage("not empty")
                .MinimumLength(8);
            RuleFor(s => s.NewPassword).NotEmpty().WithMessage("not empty")
              .MinimumLength(8);
            RuleFor(s => s.ConfirmPassword).NotEmpty().WithMessage("not empty")
         .MinimumLength(8);
            RuleFor(s => s.NewPassword).Equal(s => s.ConfirmPassword);

        }
    }
}
