using FluentValidation;
using HumanResourceService.src.Application.DTOs.Commands;

namespace HumanResourceService.src.Application.Validators;

public sealed class HResourcesCreateValidator : AbstractValidator<HResourcesCreateRequest>
{
    public HResourcesCreateValidator()
    {
        RuleFor(v => v.FullName)
            .NotEmpty()
            .NotNull()
            .Length(2, 25);

        RuleFor(v => v.PhoneNumber)
            .NotEmpty()
            .NotNull();

        RuleFor(v => v.Position)
            .NotEmpty()
            .NotNull();

        RuleFor(v => v.Subsidy)
            .NotEmpty()
            .NotNull();
    }
}