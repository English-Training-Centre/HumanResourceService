using FluentValidation;
using HumanResourceService.src.Application.DTOs.Requests;

namespace HumanResourceService.src.Application.Validators;

public sealed class EmployeeUpdateValidator : AbstractValidator<EmployeeUpdateRequest>
{
    public EmployeeUpdateValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Position)
            .NotEmpty();

        RuleFor(v => v.Subsidy)
            .NotEmpty();
    }
}