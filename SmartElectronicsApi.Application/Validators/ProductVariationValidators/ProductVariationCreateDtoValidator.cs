using FluentValidation;
using SmartElectronicsApi.Application.Dtos.ProductVariation;

namespace SmartElectronicsApi.Application.Validators.ProductVariationValidators
{
    public class ProductVariationCreateDtoValidator : AbstractValidator<ProductVariationCreateDto>
    {
        public ProductVariationCreateDtoValidator()
        {
           

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

            RuleFor(x => x.VariationName)
                .NotEmpty().WithMessage("Variation name is required.")
                .MinimumLength(3).WithMessage("Variation name must be at least 3 characters long.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID is required and must be valid.");

            RuleFor(x => x.ColorIds)
                .NotEmpty().WithMessage("At least one color ID is required.")
                .Must(colorIds => colorIds.All(id => id > 0))
                .WithMessage("All color IDs must be valid.");

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100)
                .When(x => x.DiscountPercentage.HasValue)
                .WithMessage("Discount percentage must be between 0 and 100.");

            RuleFor(s => s).Custom((c, context) =>
            {
                long maxSizeInBytes = 15 * 1024 * 1024; // 15 MB
                if (c.Images != null && c.Images.Count() > 0)
                {
                    if (c.Images.Count() > 4)
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
