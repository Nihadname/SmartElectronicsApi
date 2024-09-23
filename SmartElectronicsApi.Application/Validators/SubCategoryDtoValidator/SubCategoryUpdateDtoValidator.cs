using FluentValidation;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.SubCategoryDtoValidator
{
    internal class SubCategoryUpdateDtoValidator : AbstractValidator<SubCategoryUpdateDto>
    {
        public SubCategoryUpdateDtoValidator()
        {
            RuleFor(s => s.Name)
             .MinimumLength(4)
             .MaximumLength(120)
             .When(s => s.Name != null);

            RuleFor(s => s.Description)
                .MinimumLength(8)
                .MaximumLength(150)
                .When(s => s.Description != null);
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
