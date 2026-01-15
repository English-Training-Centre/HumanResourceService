using HumanResourceService.src.Application.DTOs.Commands;

namespace HumanResourceService.src.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<int> SaveAsync(EmployeeCreateRequest request, CancellationToken ct);
}