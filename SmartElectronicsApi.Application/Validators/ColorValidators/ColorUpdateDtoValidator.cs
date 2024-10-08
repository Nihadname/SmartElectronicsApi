using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Color;

public class ColorUpdateDtoValidator : AbstractValidator<ColorUpdateDto>
{
    public ColorUpdateDtoValidator()
    {
        RuleFor(s => s.Name).NotEmpty().WithMessage("not empty")
                 .MinimumLength(1).MaximumLength(120);
        RuleFor(s => s.Code).NotEmpty().WithMessage("not empty");
    }
}
