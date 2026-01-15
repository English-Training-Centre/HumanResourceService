using FluentValidation;
using HumanResourceService.src.Application.DTOs.Commands;

namespace HumanResourceService.src.Application.Validators;

public sealed class EmployeeCreateValidator : AbstractValidator<EmployeeCreateRequest>
{
    public EmployeeCreateValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.")
            .NotNull()
            .WithMessage("User ID is required.");

        RuleFor(v => v.Position)
            .NotEmpty()
            .NotNull();

        RuleFor(v => v.Subsidy)
            .NotEmpty()
            .NotNull();
    }
}