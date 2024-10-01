using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Subscriber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.SubscriberValidators
{
    public class SubscriberDtoValidator : AbstractValidator<SubscriberDto>
    {
        public SubscriberDtoValidator()
        {
            RuleFor(s => s.Email).NotEmpty().WithMessage("not empty")
              .MinimumLength(4).MaximumLength(120).EmailAddress();
        }
    }
}
