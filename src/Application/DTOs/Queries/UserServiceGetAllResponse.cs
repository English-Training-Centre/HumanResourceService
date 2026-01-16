namespace HumanResourceService.src.Application.DTOs.Queries;

public sealed record UserServiceGetAllResponse(
    Guid Id,
    string FullName,
    string Username,
    string PhoneNumber,
    string? Email,    
    string Role,
    string? ImageUrl,
    bool IsActive
);