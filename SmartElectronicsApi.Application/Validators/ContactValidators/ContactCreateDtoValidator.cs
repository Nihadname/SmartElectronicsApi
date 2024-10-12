using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Contact;

namespace SmartElectronicsApi.Application.Validators.ContactValidators
{
    public class ContactCreateDtoValidator : AbstractValidator<ContactCreateDto>
    {
        public ContactCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .Length(5, 100).WithMessage("Subject must be between 5 and 100 characters.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MinimumLength(10).WithMessage("Message must be at least 10 characters long.");
        }
    }
}
