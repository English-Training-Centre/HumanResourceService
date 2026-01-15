namespace HumanResourceService.src.Application.DTOs.Commands;

public sealed record UserServiceCreateRequest
(
    string FullName,
    string PhoneNumber,
    string? Email,
    string Role
);