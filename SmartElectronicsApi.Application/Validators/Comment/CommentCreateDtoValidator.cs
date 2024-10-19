using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.Comment
{
    public class CommentCreateDtoValidator : AbstractValidator<CommentCreateDto>
    {
        public CommentCreateDtoValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(500).WithMessage("Message cannot exceed 500 characters.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

            


            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Images)
                .Must(images => images.Count <= 5).WithMessage("You can upload a maximum of 5 images.");

            RuleFor(s => s).Custom((c, context) =>
            {
                long maxSizeInBytes = 15 * 1024 * 1024; // 15 MB
                if (c.Images != null && c.Images.Count() > 0)
                {
                    if (c.Images.Count() > 5)
                    {
                        context.AddFailure("Images", "Image cannot be more than 4");

                    }
                    foreach (var image in c.Images)
                    {
                        if (image == null)
                        {
                            context.AddFailure("Images", "Image cannot be null.");
                            continue;
                        }

                        if (!image.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                        {
                            context.AddFailure("Images", "Only image files are accepted.");
                        }

                        if (image.Length > maxSizeInBytes)
                        {
                            context.AddFailure("Images", $"Image '{image.FileName}' exceeds the maximum allowed size of 15 MB.");
                        }
                    }
                }
                else
                {
                    context.AddFailure("Images", "At least one image must be uploaded.");
                }
            });
        }
    }
}
