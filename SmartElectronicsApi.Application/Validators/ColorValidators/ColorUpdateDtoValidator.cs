using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Color;

internal class ColorUpdateDtoValidator : AbstractValidator<ColorUpdateDto>
{
    public ColorUpdateDtoValidator()
    {
        RuleFor(s => s.Name)
            .MinimumLength(1).MaximumLength(120)
            .When(s => !string.IsNullOrWhiteSpace(s.Name));

        RuleFor(s => s.Code)
            .MaximumLength(330)
            .When(s => !string.IsNullOrWhiteSpace(s.Code));
    }
}
