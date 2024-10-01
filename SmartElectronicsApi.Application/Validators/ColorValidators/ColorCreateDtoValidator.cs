using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.ColorValidators
{
    public class ColorCreateDtoValidator : AbstractValidator<ColorCreateDto>
    {
        public ColorCreateDtoValidator()
        {
            RuleFor(s => s.Name).NotEmpty().WithMessage("not empty")
                .MinimumLength(1).MaximumLength(120);
            RuleFor(s => s.Code).NotEmpty().WithMessage("not empty");
            
        }
    }
}
