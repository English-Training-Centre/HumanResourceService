using FluentValidation;
using Libs.Core.Internal.src.DTOs.Requests;

namespace HumanResourceService.src.Application.Validators;

public sealed class UserServiceUpdateValidator : AbstractValidator<UserUpdateRequest>
{
    public UserServiceUpdateValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.FullName)
            .NotEmpty()
            .Length(2, 25);

        RuleFor(v => v.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(v => v.RoleId)
            .NotEmpty();
    }
}