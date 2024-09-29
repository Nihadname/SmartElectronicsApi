using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.CategoryValidators
{
    public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
    {
        public CategoryUpdateDtoValidator()
        {

            RuleFor(s => s.Name)
             .MinimumLength(4)
             .MaximumLength(120)
             .When(s => s.Name != null || !string.IsNullOrWhiteSpace(s.Name));

            RuleFor(s => s.Description)
                .MinimumLength(8)
                .MaximumLength(150)
                .When(s => s.Description != null || !string.IsNullOrWhiteSpace(s.Description));

            RuleFor(s => s.Icon).MaximumLength(350)
                .When(s => s.Icon != null || !string.IsNullOrWhiteSpace(s.Icon));

            RuleFor(s => s).Custom((c, context) =>
            {
                long maxSizeInBytes = 115 * 1024 * 1024; 

                if (c.formFile != null)
                {
                    if (!c.formFile.ContentType.Contains("image/"))
                    {
                        context.AddFailure("Image", "Only image files are accepted");
                    }

                    if (c.formFile.Length > maxSizeInBytes)
                    {
                        context.AddFailure("Image", "Data storage exceeds the maximum allowed size of 15 MB");
                    }
                }
            });

        }
    }
}
