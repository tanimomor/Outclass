using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Outclass.BuildingBlocks.Application.Behaviors;
using Outclass.BuildingBlocks.Application.EventBus;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure.EventBus;
using Outclass.BuildingBlocks.Infrastructure.Middleware;
using Outclass.BuildingBlocks.Infrastructure.MultiTenancy;
using RabbitMQ.Client;
using Serilog;
using System.Reflection;
using System.Text;

namespace Outclass.BuildingBlocks.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOutclassInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        params Assembly[] assemblies)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssemblies(assemblies);

        // Multi-tenancy
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();

        // Redis
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = $"{serviceName}:";
            });
        }

        // RabbitMQ
        var rabbitHost = configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
        services.AddSingleton<IConnection>(_ =>
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitHost,
                UserName = configuration.GetValue<string>("RabbitMQ:Username") ?? "guest",
                Password = configuration.GetValue<string>("RabbitMQ:Password") ?? "guest",
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true
            };
            return factory.CreateConnection($"{serviceName}-connection");
        });
        services.AddSingleton<RabbitMqEventBus>();
        services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<RabbitMqEventBus>());

        // JWT Authentication
        var jwtKey = configuration.GetValue<string>("Jwt:Secret") ?? "OutclassPlatformSuperSecretKey2024!@#$%^&*()_+";
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetValue<string>("Jwt:Issuer") ?? "outclass",
                    ValidAudience = configuration.GetValue<string>("Jwt:Audience") ?? "outclass-api",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();

        // OpenTelemetry
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                var otlpEndpoint = configuration.GetValue<string>("OpenTelemetry:Endpoint");
                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    builder.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                }
            });

        // Health Checks
        var healthBuilder = services.AddHealthChecks();

        var pgConnection = configuration.GetConnectionString("Database");
        if (!string.IsNullOrEmpty(pgConnection))
        {
            healthBuilder.AddNpgSql(pgConnection, name: "database", tags: new[] { "db", "postgres" });
        }

        if (!string.IsNullOrEmpty(redisConnection))
        {
            healthBuilder.AddRedis(redisConnection, name: "redis", tags: new[] { "cache", "redis" });
        }

        healthBuilder.AddRabbitMQ(
            $"amqp://{configuration.GetValue<string>("RabbitMQ:Username") ?? "guest"}:{configuration.GetValue<string>("RabbitMQ:Password") ?? "guest"}@{rabbitHost}",
            name: "rabbitmq",
            tags: new[] { "messaging", "rabbitmq" });

        return services;
    }

    public static IServiceCollection AddServiceDbContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration) where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"), npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history");
                npgsqlOptions.EnableRetryOnFailure(3);
                npgsqlOptions.CommandTimeout(30);
            });
        });

        return services;
    }

    public static WebApplication UseOutclassInfrastructure(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/health");
        app.MapHealthChecks("/ready");

        return app;
    }
}
