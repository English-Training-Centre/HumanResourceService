namespace HumanResourceService.src.Application.DTOs.Commands;

public sealed record HResourcesCreateRequest
(
    string FullName,
    string PhoneNumber,
    string? Email,
    string Position,
    decimal Subsidy
);