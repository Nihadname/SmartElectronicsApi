using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.BranchValidators
{
    public class BranchCreateDtoValidator : AbstractValidator<BranchCreateDto>
    {
        public BranchCreateDtoValidator()
        {
            RuleFor(s=>s.Name).NotEmpty().MinimumLength(2).MaximumLength(120);
        }
    }
}
