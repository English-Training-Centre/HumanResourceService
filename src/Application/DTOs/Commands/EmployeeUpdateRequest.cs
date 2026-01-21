namespace HumanResourceService.src.Application.DTOs.Commands;

public sealed record EmployeeUpdateRequest
(
    Guid Id,
    string Position,
    decimal Subsidy
);
