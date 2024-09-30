using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.AddressValidators
{
    public class AddressUpdateDtoValidator : AbstractValidator<AddressUpdateDto>
    {
        public AddressUpdateDtoValidator()
        {
            RuleFor(s => s.Country).MinimumLength(2).MaximumLength(120)
                .When(s => s.Country != null || !string.IsNullOrEmpty(s.Country));
            RuleFor(s => s.City).MinimumLength(2).MaximumLength(120)
                .When(s => s.City != null || !string.IsNullOrEmpty(s.City));
            RuleFor(s => s.Street).MinimumLength(2).MaximumLength(120)
                .When(s => s.Street != null || !string.IsNullOrEmpty(s.Street));
            RuleFor(s => s.ZipCode).MinimumLength(2).MaximumLength(120)
                .When(s => s.ZipCode != null || !string.IsNullOrEmpty(s.ZipCode));
            RuleFor(s => s.AddressType)
    .IsInEnum()
    .WithMessage("Invalid address type value.")
    .When(s => s.AddressType != null);
        }
    }
}
