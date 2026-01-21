using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.DTOs.Queries;

namespace HumanResourceService.src.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<int> SaveAsync(EmployeeCreateRequest request, CancellationToken ct);
    Task<IReadOnlyList<EmployeeGetAllResponse>> GetAllAsync(CancellationToken ct);
    Task<Guid> UpdateAsync(EmployeeUpdateRequest request, CancellationToken ct);
    Task<Guid> DeleteAsync(Guid id, CancellationToken ct);
}