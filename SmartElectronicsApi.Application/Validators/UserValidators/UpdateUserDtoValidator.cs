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
                .MaximumLength(100).WithMessage("max is 100").When(s => s.UserName != null);

            RuleFor(s => s.FullName).MaximumLength(100).WithMessage("max is 100").When(s => s.UserName != null);

           
            RuleFor(x => x.PhoneNumber)
           .Matches(@"^(\+?\d{1,4}?)[\s.-]?\(?\d{1,4}?\)?[\s.-]?\d{1,4}[\s.-]?\d{1,9}$")
           .WithMessage("Invalid phone number format.").When(s => s.UserName != null);
        }
    }
}
