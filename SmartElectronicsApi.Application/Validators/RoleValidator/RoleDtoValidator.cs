using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.RoleValidator
{
    public class RoleDtoValidator : AbstractValidator<RoleDto>
    {
        public RoleDtoValidator()
        {
            RuleFor(s => s.Name).NotEmpty().WithMessage("not empty")
                .MinimumLength(1).MaximumLength(120);
        }
    }
}
