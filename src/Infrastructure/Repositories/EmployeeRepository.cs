using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.Interfaces;
using Npgsql;

namespace HumanResourceService.src.Infrastructure.Repositories;

public sealed class EmployeeRepository(IPostgresDB db, ILogger<EmployeeRepository> logger) : IEmployeeRepository
{
    private readonly IPostgresDB _db = db;
    private readonly ILogger<EmployeeRepository> _logger = logger;

    public async Task<int> SaveAsync(EmployeeCreateRequest request, CancellationToken ct)
    {
        const string sql = @"INSERT INTO tbEmployees
                    (user_id, position, subsidy) 
                    VALUES(@UserId, @Position, @Subsidy);";

        try
        {
            var result = await _db.ExecuteAsync(sql, request, ct);

            return result == 0
                ? 0
                : 1;
        }
        catch (PostgresException pgEx) when (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            _logger.LogError(pgEx, " - Already exists.");
            return 2;
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, " - Unexpected PostgreSQL Error.");
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - Unexpected error during transaction operation.");
            return 0;
        }
    }
}