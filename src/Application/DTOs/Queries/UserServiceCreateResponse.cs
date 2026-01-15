namespace HumanResourceService.src.Application.DTOs.Queries;

public sealed record UserServiceCreateResponse
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
}