using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.DTOs.Queries;

namespace HumanResourceService.src.Application.Interfaces;

public interface IUserGrpcServiceClient
{
    Task<UserServiceCreateResponse> CreateAsync(UserServiceCreateRequest request, CancellationToken ct);
}