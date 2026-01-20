namespace HumanResourceService.src.Application.DTOs.Commands;

public sealed record HResourcesCreateRequest
(
    string FullName,
    string PhoneNumber,
    string? Email,
    Guid RoleId,
    string Position,
    decimal Subsidy
);