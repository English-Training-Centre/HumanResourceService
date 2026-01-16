using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.DTOs.Queries;

namespace HumanResourceService.src.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<int> SaveAsync(EmployeeCreateRequest request, CancellationToken ct);
    Task<IReadOnlyList<EmployeeGetAllResponse>> GetAllAsync(CancellationToken ct);
}