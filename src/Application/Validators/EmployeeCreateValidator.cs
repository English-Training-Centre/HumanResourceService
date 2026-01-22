using FluentValidation;
using HumanResourceService.src.Application.DTOs.Requests;

namespace HumanResourceService.src.Application.Validators;

public sealed class EmployeeCreateValidator : AbstractValidator<EmployeeCreateRequest>
{
    public EmployeeCreateValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty();

        RuleFor(v => v.Position)
            .NotEmpty();

        RuleFor(v => v.Subsidy)
            .NotEmpty();
    }
}