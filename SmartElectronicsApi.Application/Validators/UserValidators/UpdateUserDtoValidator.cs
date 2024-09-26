using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.UserValidators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(s => s.UserName)
                .MaximumLength(100).WithMessage("max is 100").When(s => !string.IsNullOrWhiteSpace(s.Email));

            RuleFor(s => s.FullName).MaximumLength(100).WithMessage("max is 100").When(s => !string.IsNullOrWhiteSpace(s.Email));

            RuleFor(s => s.Email).EmailAddress().When(s => !string.IsNullOrWhiteSpace(s.Email));


        }
    }
}
