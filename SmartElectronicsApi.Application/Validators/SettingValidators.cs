using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators
{
    public class SettingValidators : AbstractValidator<SettingDto>
    {
        public SettingValidators()
        {
            RuleFor(s => s.Key).NotEmpty().WithMessage("not empty")
             .MinimumLength(4).MaximumLength(120);
            RuleFor(s => s.Value).NotEmpty().WithMessage("not empty")
              .MinimumLength(8).MaximumLength(150);
        }
    }
}
