using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.DTOs.Queries;
using HumanResourceService.src.Application.Interfaces;

namespace HumanResourceService.src.Application.Handlers;

public sealed class EmployeeHandler(IEmployeeRepository employeeRepository, IUserGrpcServiceClient userClient, ILogger<EmployeeHandler> logger) : IEmployeeHandler
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly IUserGrpcServiceClient _userClient = userClient;
    private readonly ILogger<EmployeeHandler> _logger = logger;

    public async Task<HResourcesCreateResponse> CreateAsync(HResourcesCreateRequest request, CancellationToken ct)
    {
        var userClientRequest = new UserServiceCreateRequest
        (
            request.FullName,
            request.PhoneNumber,
            request.Email,
            "employee"
        );

        try
        {
            var userClient = await _userClient.CreateAsync(userClientRequest, ct);

            if (!userClient.IsSuccess || userClient.UserId.Equals(Guid.Empty) || userClient.Username is null || userClient.Password is null)
            {
                return new HResourcesCreateResponse { IsSuccess = false, Message = userClient.Message };
            }

            var employeeRequest = new EmployeeCreateRequest
            (
                userClient.UserId,
                request.Position,
                request.Subsidy
            );

            var employee = await _employeeRepository.SaveAsync(employeeRequest, ct);

            return employee switch
            {
                1 => new HResourcesCreateResponse
                        {
                            IsSuccess = true,
                            Message = userClient.Message,
                            Username = userClient.Username,
                            Password = userClient.Password
                        },

                _ => new HResourcesCreateResponse { IsSuccess = false }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - An unexpected error occurred...");
            return new HResourcesCreateResponse { IsSuccess = false };
        }
    }
}