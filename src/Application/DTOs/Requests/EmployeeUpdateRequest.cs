namespace HumanResourceService.src.Application.DTOs.Requests;

public sealed record EmployeeUpdateRequest
(
    Guid Id,
    string Position,
    decimal Subsidy
);
