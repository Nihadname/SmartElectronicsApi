using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.UserValidators
{
    public class UserUpdateImageDtoValidator : AbstractValidator<UserUpdateImageDto>
    {
        public UserUpdateImageDtoValidator()
        {
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
