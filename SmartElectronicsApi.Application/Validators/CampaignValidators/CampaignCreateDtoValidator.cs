using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Campaign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.CampaignValidators
{
    public class CampaignCreateDtoValidator : AbstractValidator<CreateCampaignDto>
    {
        public CampaignCreateDtoValidator()
        {
            RuleFor(s=>s.Title).NotEmpty().MinimumLength(2).MaximumLength(100);
            RuleFor(x => x.StartDate)
           .NotNull()
     .GreaterThanOrEqualTo(DateTime.UtcNow)
     .WithMessage("StartDate must be in the future.");

            RuleFor(x => x.EndDate)
                 .NotNull()
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("StartDate must be in the future.");
            RuleFor(x => x)
           .Must(x => x.StartDate <= x.EndDate);
            RuleFor(s => s.DiscountPercentage)
    .Must(value => !value.HasValue || value.Value <= 100)
    .WithMessage("Discount percentage cannot be greater than 100.");
            RuleFor(s=>s.Description).MinimumLength(2).MaximumLength(100).When(value=>!string.IsNullOrWhiteSpace(value.Description));
            RuleFor(s => s).Custom((c, context) =>
            {
                long maxSizeInBytes = 115 * 1024 * 1024;
                if (c.formFile == null || !c.formFile.ContentType.Contains("image/"))
                {
                    context.AddFailure("Image", "Only image files are accepted");
                }

                if (c.formFile != null && c.formFile.Length > maxSizeInBytes)
                {
                    context.AddFailure("Image", "Data storage exceeds the maximum allowed size of 15 MB");
                }

            });
        }
    }
}
