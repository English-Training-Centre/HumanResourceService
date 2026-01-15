namespace HumanResourceService.src.Application.DTOs.Queries;

public sealed record EmployeeGetAllResponse
(
    Guid Id,
    string FullName,
    string Username,
    string PhoneNumber,
    string? Email,
    string Role,
    bool IsActive,
    string Position,
    decimal Subsidy
);