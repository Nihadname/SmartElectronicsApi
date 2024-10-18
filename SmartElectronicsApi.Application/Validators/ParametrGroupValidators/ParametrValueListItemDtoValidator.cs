using FluentValidation;
using SmartElectronicsApi.Application.Dtos.ParametrValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.ParametrGroupValidators
{
    public class ParametrValueListItemDtoValidator : AbstractValidator<ParametrValueListItemDto>
    {
        public ParametrValueListItemDtoValidator()
        {
            // Validate Type
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.");

            // Validate Value
            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Value is required.");
        }
    }
}
