namespace HumanResourceService.src.Application.DTOs.Queries;

public sealed record ResponseDTO
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
}