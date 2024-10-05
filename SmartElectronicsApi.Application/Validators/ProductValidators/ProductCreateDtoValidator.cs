using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Product;
using System;

namespace SmartElectronicsApi.Application.Validators.ProductValidators
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            // Name: Required, at least 3 characters and at most 100 characters
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .Length(3, 100).WithMessage("Product name must be between 3 and 100 characters long.");

            // Description: Optional, but if provided, must be at least 10 characters and at most 500 characters
            RuleFor(x => x.Description)
                  .NotEmpty().WithMessage("Product Description is required.")
                .MaximumLength(500).WithMessage("Description cannot be more than 500 characters.");



            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.").NotEmpty();



            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.")
                .When(x => x.DiscountPercentage > 0);


            RuleFor(x => x.DiscountedPrice)
      .LessThanOrEqualTo(x => x.Price).WithMessage("Discounted price must be lower than or equal to the original price.")
      .When(x => x.DiscountPercentage != null && x.DiscountPercentage > 0);



            RuleFor(x => x.isNew)
                .NotNull().WithMessage("IsNew is required.");


            RuleFor(x => x.ProductCode)
                .Length(5, 20).WithMessage("Product code must be between 5 and 20 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ProductCode));



            RuleFor(x => x.IsDealOfTheWeek)
                .NotNull().WithMessage("IsDealOfTheWeek is required.");

       
            RuleFor(x => x.IsFeatured)
                .NotNull().WithMessage("IsFeatured is required.");
            RuleFor(x => x.CategoryId)
                .NotNull();
            RuleFor(x => x.BrandId)
               .NotNull();
            RuleFor(x => x.SubcategoryId)
               .NotNull();
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be at least 0.");

        
            RuleFor(x => x.ViewCount)
                .GreaterThanOrEqualTo(0).WithMessage("View count must be 0 or higher.");

            
            RuleFor(x => x.CreatedTime)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Created time must be in the past.")
                .When(x => x.CreatedTime.HasValue);
        }
    }
}
