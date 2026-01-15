namespace HumanResourceService.src.Application.DTOs.Queries;

public sealed record HResourcesCreateResponse
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
}