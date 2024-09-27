using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Address;

namespace SmartElectronicsApi.Application.Validators.AddressValidators
{
    public class AddressCreateDtoValidator : AbstractValidator<AddressCreateDto>
    {
        public AddressCreateDtoValidator()
        {
            RuleFor(s => s.Country).NotEmpty().WithMessage("not empty")
             .MinimumLength(2).MaximumLength(120);
            RuleFor(s => s.City).NotEmpty().WithMessage("not empty")
             .MinimumLength(2).MaximumLength(130);
            RuleFor(s => s.Street).NotEmpty().WithMessage("not empty")
          .MinimumLength(2).MaximumLength(160);
            RuleFor(s => s.ZipCode).NotEmpty().WithMessage("not empty")
          .MinimumLength(2).MaximumLength(130);
            RuleFor(s => s.AddressType)
     .IsInEnum()
     .WithMessage("Invalid address type value.");
           

            

        }
    }
}
