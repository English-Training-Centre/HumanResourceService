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
        try
        {
            var parameter = new HResourcesCreateRequest
            (
                request.FullName,
                request.PhoneNumber,
                request.Email,                
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
            throw new RpcException(new Status(StatusCode.Internal, "Failed to create user"));
        }
    }
}