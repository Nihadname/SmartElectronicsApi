using FluentValidation;
using SmartElectronicsApi.Application.Dtos.ParametrGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.ParametrGroupValidators
{
    internal class ParametrGroupCreateDtoValidator : AbstractValidator<ParametrGroupCreateDto>
    {
        public ParametrGroupCreateDtoValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 100).WithMessage("Name must be between 3 and 100 characters.");

            // Validate ProductId
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

            // Validate ParametrValues
            RuleForEach(x => x.parametrValues).SetValidator(new ParametrValueListItemDtoValidator());
        }
    }
}
