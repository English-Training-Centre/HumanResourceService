using Grpc.Core;
using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.DTOs.Queries;
using HumanResourceService.src.Application.Interfaces;
using UserService;

namespace HumanResourceService.src.Infrastructure.Services;

public sealed class UserGrpcServiceClient(UsersGrpc.UsersGrpcClient client, ILogger<UserGrpcServiceClient> logger) : IUserGrpcServiceClient
{
    private readonly UsersGrpc.UsersGrpcClient _client = client;
    private readonly ILogger<UserGrpcServiceClient> _logger = logger;

    public async Task<UserServiceCreateResponse> Create(UserServiceCreateRequest request, CancellationToken ct)
    {
        var grpcRequest = new GrpcUserCreateRequest
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Role = request.Role
        };

        GrpcUserAuthCreatedResponse grpcResponse;
        try
        {
            grpcResponse = await _client.CreateAsync(grpcRequest, new CallOptions(
                deadline: DateTime.UtcNow.AddSeconds(10),
                cancellationToken: ct
            ));
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "- UserGrpcServiceClient -> CreateAsync(...)");
            return new UserServiceCreateResponse { IsSuccess = false };
        }

        var isSuccess = grpcResponse.IsSuccess;

        if (!isSuccess || string.IsNullOrWhiteSpace(grpcResponse.UserId) ||
            !Guid.TryParse(grpcResponse.UserId, out var userId))
        {
            return new UserServiceCreateResponse { IsSuccess = false };
        }

        return new UserServiceCreateResponse
        {
            IsSuccess = isSuccess,
            Message = grpcResponse.Message,
            UserId = userId,
            Username = grpcResponse.Username ?? string.Empty,
            Password = grpcResponse.Password ?? string.Empty
        };
    }

    public async Task<IReadOnlyList<UserServiceGetAllResponse>> GetAllByIds(IReadOnlyCollection<Guid> ids, CancellationToken ct)
    {
        if (ids == null || ids.Count == 0)
            return [];

        var grpcRequest = new GrpcGetUsersByIdsRequest
        {
            Ids = { ids.Select(id => id.ToString()) }
        };

        GrpcUserGetAllListResponse grpcResponse;
        try
        {
            grpcResponse = await _client.GetAllByIdsAsync(grpcRequest, new CallOptions(
                deadline: DateTime.UtcNow.AddSeconds(10),
                cancellationToken: ct
            ));
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "- UserGrpcServiceClient -> GetAllByIds(...)");
            return [];
        }

        return [.. grpcResponse.Users
            .Select(u => new UserServiceGetAllResponse(
                Guid.Parse(u.Id),
                u.FullName,
                u.Username,
                u.PhoneNumber,
                u.Email.Length == 0 ? null : u.Email,
                u.Role,
                string.IsNullOrEmpty(u.ImageUrl) ? null : u.ImageUrl,
                u.IsActive
            ))];
    }
}