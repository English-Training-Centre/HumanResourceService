namespace HumanResourceService.src.Application.DTOs.Commands;

public sealed record EmployeeCreateRequest
(
    Guid UserId,
    string Position,
    decimal Subsidy
);