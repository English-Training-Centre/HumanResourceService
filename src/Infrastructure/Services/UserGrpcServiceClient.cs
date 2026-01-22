using Grpc.Core;
using Libs.Core.Internal.Protos.UserService;
using Libs.Core.Internal.src.DTOs.Requests;
using Libs.Core.Internal.src.DTOs.Responses;
using Libs.Core.Internal.src.Interfaces;
using Libs.Core.Shared.src.DTOs.Responses;

namespace HumanResourceService.src.Infrastructure.Services;

public sealed class UserGrpcServiceClient(UsersGrpc.UsersGrpcClient client, ILogger<UserGrpcServiceClient> logger) : IUserGrpcService
{
    private readonly UsersGrpc.UsersGrpcClient _client = client;
    private readonly ILogger<UserGrpcServiceClient> _logger = logger;

    public async Task<UserCreatedResponse> CreateAsync(UserCreateRequest request, CancellationToken ct)
    {
        var grpcRequest = new GrpcUserCreateRequest
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            RoleId = request.RoleId.ToString()
        };

        GrpcUserCreatedResponse grpcResponse;
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
            return new UserCreatedResponse { IsSuccess = false, Message = "Failed to create user." };
        }

        var isSuccess = grpcResponse.IsSuccess;

        if (!isSuccess || string.IsNullOrWhiteSpace(grpcResponse.UserId) ||
            !Guid.TryParse(grpcResponse.UserId, out var userId))
        {
            return new UserCreatedResponse { IsSuccess = false, Message = grpcResponse.Message };
        }

        return new UserCreatedResponse
        {
            IsSuccess = isSuccess,
            Message = grpcResponse.Message,
            UserId = userId,
            Username = grpcResponse.Username ?? string.Empty,
            Password = grpcResponse.Password ?? string.Empty
        };
    }

    public async Task<IReadOnlyList<UserGetAllResponse>> GetAllByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
    {
        if (ids == null || ids.Count == 0)
            return [];

        var grpcRequest = new GrpcUserByIdsRequest
        {
            Ids = { ids.Select(id => id.ToString()) }
        };

        GrpcListUserGetAllResponse grpcResponse;
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
            .Select(u => new UserGetAllResponse
            {
                Id = Guid.Parse(u.Id),
                FullName = u.FullName,
                Username = u.Username,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email.Length == 0 ? null : u.Email,
                Role = u.Role,
                ImageUrl = string.IsNullOrEmpty(u.ImageUrl) ? null : u.ImageUrl,
                IsActive = u.IsActive
            })];
    }

    public async Task<ResponseDTO> UpdateAsync(UserUpdateRequest request, CancellationToken ct)
    {
        var grpcRequest = new GrpcUserUpdateRequest
        {
            Id = request.Id.ToString(),
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            RoleId = request.RoleId.ToString()
        };

        GrpcUserResponseDTO grpcResponse;
        try
        {
            grpcResponse = await _client.UpdateAsync(grpcRequest, new CallOptions(
                deadline: DateTime.UtcNow.AddSeconds(10),
                cancellationToken: ct
            ));
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "- UserGrpcServiceClient -> UpdateAsync(...)");
            return new ResponseDTO { IsSuccess = false, Message = "Failed to update user." };
        }

        return new ResponseDTO
        {
            IsSuccess = grpcResponse.IsSuccess,
            Message = grpcResponse.Message
        };
    }

    public async Task<ResponseDTO> DeleteAsync(Guid id, CancellationToken ct)
    {
        var grpcRequest = new GrpcUserDeleteRequest
        {
            Id = id.ToString()
        };

        GrpcUserResponseDTO grpcResponse;
        try
        {
            grpcResponse = await _client.DeleteAsync(grpcRequest, new CallOptions(
                deadline: DateTime.UtcNow.AddSeconds(10),
                cancellationToken: ct
            ));
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "- UserGrpcServiceClient -> DeleteAsync(...)");
            return new ResponseDTO { IsSuccess = false, Message = "Failed to delete user." };
        }

        return new ResponseDTO
        {
            IsSuccess = grpcResponse.IsSuccess,
            Message = grpcResponse.Message
        };
    }
}