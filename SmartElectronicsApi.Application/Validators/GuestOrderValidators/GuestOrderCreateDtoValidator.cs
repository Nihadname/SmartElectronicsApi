﻿using FluentValidation;
using SmartElectronicsApi.Application.Dtos.GuestOrder;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.GuestOrderValidators
{
    public class GuestOrderCreateDtoValidator : AbstractValidator<GuestOrderCreateDto>
    {
        public GuestOrderCreateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("GuestOrder name is required.")
                .Length(3, 200).WithMessage("GuestOrder name must be between 3 and 200 characters long.");
           
            RuleFor(x => x.Age)
                .GreaterThan(18).NotEmpty();
            RuleFor(x => x.Address)
               .NotEmpty().WithMessage("GuestOrder Address is required.")
               .Length(3, 200).WithMessage("GuestOrder Address must be between 3 and 200 characters long.");
            RuleFor(x => x.EmailAdress)
              .NotEmpty().WithMessage("GuestOrder Address is required.")
              .Length(3, 200).WithMessage("GuestOrder Address must be between 3 and 200 characters long.").EmailAddress();
            RuleFor(x => x.PhoneNumber)
             .NotEmpty().WithMessage("GuestOrder PhoneNumber is required.")
             .Length(3, 200).WithMessage("GuestOrder PhoneNumber must be between 3 and 200 characters long.");
            RuleFor(x => x.PhoneNumber)
             .NotEmpty().WithMessage("Phone number is required.")
           .Matches(@"^(\+?\d{1,4}?)[\s.-]?\(?\d{1,4}?\)?[\s.-]?\d{1,4}[\s.-]?\d{1,9}$")
           .WithMessage("Invalid phone number format.");
            RuleFor(x => x.ExtraInformation)
     .Length(3, 200)
     .WithMessage("GuestOrder name must be between 3 and 200 characters long.")
     .When(x => !string.IsNullOrEmpty(x.ExtraInformation));


            RuleFor(x => x.PurchasedProductId)
                .GreaterThan(18).NotEmpty();
        }
    }
}
