namespace HumanResourceService.src.Application.DTOs.Responses;

public sealed record EmployeeGetAllResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Position { get; init; } = string.Empty;
    public long Subsidy { get; init; }
}