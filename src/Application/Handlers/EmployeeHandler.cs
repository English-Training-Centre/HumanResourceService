using HumanResourceService.src.Application.DTOs.Requests;
using HumanResourceService.src.Application.Interfaces;
using Libs.Core.Internal.src.DTOs.Requests;
using Libs.Core.Internal.src.Interfaces;
using Libs.Core.Public.src.DTOs.Requests;
using Libs.Core.Public.src.DTOs.Responses;
using Libs.Core.Public.src.Interfaces;
using Libs.Core.Shared.src.DTOs.Responses;

namespace HumanResourceService.src.Application.Handlers;

public sealed class EmployeeHandler(IEmployeeRepository employeeRepository, IUserGrpcService userClient, ILogger<EmployeeHandler> logger) : IHResourceGrpcService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly IUserGrpcService _userClient = userClient;
    private readonly ILogger<EmployeeHandler> _logger = logger;

    public async Task<HResourcesCreateResponse> CreateAsync(HResourcesCreateRequest request, CancellationToken ct)
    {
        var userClientRequest = new UserCreateRequest
        (
            request.FullName,
            request.PhoneNumber,
            request.Email,
            request.RoleId
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

                _ => new HResourcesCreateResponse { IsSuccess = false, Message = userClient.Message }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - An unexpected error occurred...");
            return new HResourcesCreateResponse { IsSuccess = false, Message = "An unexpected error occurred..." };
        }
    }

    public async Task<IReadOnlyList<HResourcesGetAllResponse>> GetAllWithUserAsync(CancellationToken ct)
    {
        var employees = (await _employeeRepository.GetAllAsync(ct)).ToList();

        if (employees.Count == 0)
            return [];

        // Extrair todos os UserIds
        var userIds = employees
            .Select(e => e.UserId)
            .Distinct() // elimina duplicados
            .ToList();

        var users = await _userClient.GetAllByIdsAsync(userIds, ct);

        // Mapear usuários por Id para merge rápido
        var usersById = users.ToDictionary(u => u.Id, u => u);

        // lista combinada
        var result = employees.Select(e =>
        {
            usersById.TryGetValue(e.UserId, out var user);

            return new HResourcesGetAllResponse
            {
                Id = e.Id,
                FullName = user!.FullName,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = user.Role,
                ImageUrl = user.ImageUrl,
                IsActive = user.IsActive,
                Position = e.Position,
                Subsidy = e.Subsidy
            };
        })
        .OrderBy(r => r.FullName)
        .ToList();

        return result;
    }

    public async Task<ResponseDTO> UpdateAsync(HResourcesUpdateRequest request, CancellationToken ct)
    {
        var employeeRequest = new EmployeeUpdateRequest
        (
            request.Id,
            request.Position,
            request.Subsidy
        );

        try
        {
            var userId = await _employeeRepository.UpdateAsync(employeeRequest, ct);

            if (userId.Equals(Guid.Empty))
            {
                return new ResponseDTO { IsSuccess = false, Message = "Failed to update employee. User ID is null..." };
            }
            else
            {
                var userClientRequest = new UserUpdateRequest
                (
                    userId,
                    request.FullName,
                    request.Email,
                    request.PhoneNumber,                    
                    request.RoleId
                );

                var userClient = await _userClient.UpdateAsync(userClientRequest, ct);

                return new ResponseDTO
                {
                    IsSuccess = userClient.IsSuccess,
                    Message = userClient.Message
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - An unexpected error occurred...");
            return new ResponseDTO { IsSuccess = false, Message = "An unexpected error occurred..." };
        }
    }

    public async Task<ResponseDTO> DeleteAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Employee ID is empty or invalid");
            return new ResponseDTO { IsSuccess = false, Message = "Invalid employee ID." };
        }

        try
        {
            var userId = await _employeeRepository.DeleteAsync(id, ct);

            if (userId.Equals(Guid.Empty))
            {
                return new ResponseDTO { IsSuccess = false, Message = "Failed to delete. User ID is null..." };
            }
            else
            {
                var userClient = await _userClient.DeleteAsync(userId, ct);

                return new ResponseDTO
                {
                    IsSuccess = userClient.IsSuccess,
                    Message = userClient.Message
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - An unexpected error occurred...");
            return new ResponseDTO { IsSuccess = false, Message = "An unexpected error occurred..." };
        }
    }
}