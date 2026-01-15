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

    public async Task<UserServiceCreateResponse> CreateAsync(UserServiceCreateRequest request, CancellationToken ct)
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
}