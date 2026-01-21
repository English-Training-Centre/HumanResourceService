using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.DTOs.Queries;

namespace HumanResourceService.src.Application.Interfaces;

public interface IEmployeeHandler
{
    Task<HResourcesCreateResponse> CreateAsync(HResourcesCreateRequest request, CancellationToken ct);
    Task<IReadOnlyList<EmployeeWithUserGetResponse>> GetAllWithUserAsync(CancellationToken ct);
    Task<ResponseDTO> UpdateAsync(HResourcesUpdateRequest request, CancellationToken ct);
    Task<ResponseDTO> DeleteAsync(Guid id, CancellationToken ct);
}