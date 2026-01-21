using Dapper;
using HumanResourceService.src.Application.DTOs.Commands;
using HumanResourceService.src.Application.DTOs.Queries;
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

    public async Task<IReadOnlyList<EmployeeGetAllResponse>> GetAllAsync(CancellationToken ct)
    {
        const string sql = @"SELECT id As Id, user_id As UserId, position As Position, subsidy As Subsidy FROM tbEmployees;";

        try
        {
            return (await _db.QueryAsync<EmployeeGetAllResponse>(sql, new{}, ct)).AsList();
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, " - Unexpected PostgreSQL Error");
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - Unexpected error during transaction operation.");
            return [];
        }
    }

    public async Task<Guid> UpdateAsync(EmployeeUpdateRequest request, CancellationToken ct)
    {
        const string sql = @"
        WITH updated AS 
        (
            UPDATE tbEmployees
            SET
                position = CASE WHEN position IS DISTINCT FROM @Position THEN @Position ELSE position END,
                subsidy  = CASE WHEN subsidy  IS DISTINCT FROM @Subsidy  THEN @Subsidy  ELSE subsidy  END
            WHERE id = @Id
        )
        SELECT user_id
        FROM tbEmployees
        WHERE id = @Id;";

        try
        {
            return await _db.ExecuteScalarAsync<Guid>(
                sql,
                ct,
                new NpgsqlParameter("@Position", request.Position),
                new NpgsqlParameter("@Subsidy", request.Subsidy),
                new NpgsqlParameter("@Id", request.Id)
            );
        }
        catch (PostgresException pgEx) when (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            _logger.LogError(pgEx, " - Already exists.");
            return Guid.Empty;
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, " - Unexpected PostgreSQL Error.");
            return Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - Unexpected error during transaction operation.");
            return Guid.Empty;
        }
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"DELETE FROM tbEmployees WHERE id = @Id RETURNING user_id;";

        try
        {
            return await _db.ExecuteScalarAsync<Guid>(
                sql,
                ct,
                new NpgsqlParameter("@Id", id)
            );
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, " - Unexpected PostgreSQL Error.");
            return Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - Unexpected error during transaction operation.");
            return Guid.Empty;
        }
    }
}