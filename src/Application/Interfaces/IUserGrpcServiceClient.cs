using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.DTOs.Queries;

namespace HumanResourceService.src.Application.Interfaces;

public interface IUserGrpcServiceClient
{
    Task<UserServiceCreateResponse> Create(UserServiceCreateRequest request, CancellationToken ct);
    Task<IReadOnlyList<UserServiceGetAllResponse>> GetAllByIds(IReadOnlyCollection<Guid> ids, CancellationToken ct);
    Task<ResponseDTO> Update(UserServiceUpdateRequest request, CancellationToken ct);
    Task<ResponseDTO> Delete(Guid id, CancellationToken ct);
}