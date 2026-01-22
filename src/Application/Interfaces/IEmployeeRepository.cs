using HumanResourceService.src.Application.DTOs.Requests;
using HumanResourceService.src.Application.DTOs.Responses;

namespace HumanResourceService.src.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<int> SaveAsync(EmployeeCreateRequest request, CancellationToken ct);
    Task<IReadOnlyList<EmployeeGetAllResponse>> GetAllAsync(CancellationToken ct);
    Task<Guid> UpdateAsync(EmployeeUpdateRequest request, CancellationToken ct);
    Task<Guid> DeleteAsync(Guid id, CancellationToken ct);
}