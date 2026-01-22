namespace HumanResourceService.src.Application.DTOs.Requests;

public sealed record EmployeeCreateRequest
(
    Guid UserId,
    string Position,
    decimal Subsidy
);