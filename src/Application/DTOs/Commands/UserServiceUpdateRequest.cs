namespace HumanResourceService.src.Application.DTOs.Commands;

public sealed record UserServiceUpdateRequest
(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    Guid RoleId
);