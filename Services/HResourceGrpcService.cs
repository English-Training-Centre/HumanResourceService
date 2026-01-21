using Grpc.Core;
using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.Interfaces;

namespace HumanResourceService.Services;

public sealed class HResourceGrpcService(IEmployeeHandler employeeHandler, ILogger<HResourceGrpcService> logger) : HumanResourcesGrpc.HumanResourcesGrpcBase
{
    private readonly IEmployeeHandler _employeeHandler = employeeHandler;
    private readonly ILogger<HResourceGrpcService> _logger = logger;

    public override async Task<GrpcHResourcesCreateResponse> Create(GrpcHResourcesCreateRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.RoleId, out var roleId))
        {
            throw new RpcException(new Status(StatusCode.Internal, "Role ID Invalid..."));
        }

        try
        {
            var parameter = new HResourcesCreateRequest
            (
                request.FullName,
                request.PhoneNumber,
                request.Email, 
                roleId,               
                request.Position,
                request.Subsidy
            );

            var result = await _employeeHandler.CreateAsync(parameter, context.CancellationToken);

            var protoResponse = new GrpcHResourcesCreateResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Username = result.Username ?? "",
                Password = result.Password ?? ""
            };

            return protoResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: HResourceGrpcService -> Create(....)");
            throw new RpcException(new Status(StatusCode.Internal, "Failed to create employee"));
        }
    }

    public override async Task<GrpcEmployeeGetAllResponse> GetAll(Google.Protobuf.WellKnownTypes.Empty request, ServerCallContext context)
    {
        try
        {
            var result = await _employeeHandler.GetAllWithUserAsync(context.CancellationToken);

            var protoResponse = new GrpcEmployeeGetAllResponse();
            protoResponse.Employees.AddRange(result.Select(u => new GrpcEmployeeWithUserGetResponse
            {
                Id = u.Id.ToString(),
                FullName = u.FullName ?? string.Empty,
                Username = u.Username ?? string.Empty,
                PhoneNumber = u.PhoneNumber ?? string.Empty,
                Email = u.Email ?? string.Empty,                
                Role = u.Role ?? string.Empty,
                ImageUrl = u.ImageUrl ?? string.Empty,
                IsActive = u.IsActive,
                Position = u.Position ?? string.Empty,
                Subsidy = u.Subsidy
            }));

            return protoResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: HResourceGrpcService -> GetAll(....)");
            throw new RpcException(new Status(StatusCode.Internal, "Failed to get all"));
        }
    }

    public override async Task<GrpcHResourcesResponseDTO> Update(GrpcHResourcesUpdateRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.Internal, "Employee ID Invalid..."));
        }

        if (!Guid.TryParse(request.RoleId, out var roleId))
        {
            throw new RpcException(new Status(StatusCode.Internal, "Role ID Invalid..."));
        }

        try
        {
            var parameter = new HResourcesUpdateRequest
            (
                id,
                request.FullName,
                request.PhoneNumber,
                request.Email, 
                roleId,               
                request.Position,
                request.Subsidy
            );

            var result = await _employeeHandler.UpdateAsync(parameter, context.CancellationToken);

            var protoResponse = new GrpcHResourcesResponseDTO
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message
            };

            return protoResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: HResourceGrpcService -> Update(....)");
            throw new RpcException(new Status(StatusCode.Internal, "Failed to update employee"));
        }
    }

    public override async Task<GrpcHResourcesResponseDTO> Delete(GrpcHResourcesDeleteRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.Internal, "Employee ID Invalid..."));
        }

        try
        {
            var result = await _employeeHandler.DeleteAsync(id, context.CancellationToken);

            var protoResponse = new GrpcHResourcesResponseDTO
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message
            };

            return protoResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: HResourceGrpcService -> Delete(....)");
            throw new RpcException(new Status(StatusCode.Internal, "Failed to delete employee"));
        }
    }
}