using FluentValidation;
using FluentValidation.AspNetCore;
using Npgsql;
using HumanResourceService.src.Application.Interfaces;
using HumanResourceService.src.Infrastructure.Persistence;
using System.IO.Compression;
using Polly;
using Grpc.Net.Compression;
using HumanResourceService.src.Infrastructure.Services;
using HumanResourceService.src.Infrastructure.Repositories;
using HumanResourceService.src.Application.Handlers;
using HumanResourceService.Services;
using Libs.Core.Internal.src.Interfaces;
using Libs.Core.Internal.Protos.UserService;
using Libs.Core.Public.src.Interfaces;

namespace HumanResourceService.src.Configuration;

public static class MiddlewareConfig
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var logger = LoggerFactory.Create(s => s.AddConsole()).CreateLogger<Program>();
        try
        {
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            services.AddFluentValidationAutoValidation();

            services.AddSingleton(nd =>
            {
                var builder = new NpgsqlDataSourceBuilder(
                    PostgresDB.BuildConnectionStringFromEnvironment()
                );
                builder.UseLoggerFactory(nd.GetRequiredService<ILoggerFactory>());
                return builder.Build();
            });

            services.AddHealthChecks()
                    .AddNpgSql(nd => nd.GetRequiredService<NpgsqlDataSource>());

            services.AddScoped<IPostgresDB, PostgresDB>();
            services.AddScoped<IUserGrpcService, UserGrpcServiceClient>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IHResourceGrpcService, EmployeeHandler>();

            services.AddGrpcClient<UsersGrpc.UsersGrpcClient>(op =>
            {
                op.Address = new Uri(configuration["GrpcServices:UserService"] ?? "http://localhost:5284");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
                new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true
                })
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(500)))
            .ConfigureChannel(channelOptions =>
            {
                channelOptions.MaxReceiveMessageSize = 10 * 1024 * 1024; // 10 MB
                channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;     // 5 MB
                channelOptions.CompressionProviders = [new GzipCompressionProvider(CompressionLevel.Fastest)];
            });

            services.AddGrpc();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while configuring Middleware");
        }
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.MapGrpcService<HResourceGrpcService>();

        return app;
    }
}