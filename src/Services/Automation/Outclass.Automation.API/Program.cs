using Hangfire;
using Hangfire.PostgreSql;
using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure;
using Outclass.BuildingBlocks.Infrastructure.MultiTenancy;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Automation.Domain.Entities;
using Outclass.Automation.Infrastructure.EventHandlers;
using Outclass.Automation.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext().Enrich.WithProperty("Service", "AutomationService")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {Message:lj}{NewLine}{Exception}"));

builder.Services.AddControllers();
builder.Services.AddOutclassInfrastructure(builder.Configuration, "automation-service",
    typeof(Outclass.Automation.Application.Commands.CreateAutomationRuleCommand).Assembly);
builder.Services.AddServiceDbContext<AutomationDbContext>(builder.Configuration);

builder.Services.AddScoped<IRepository<AutomationRule>, EfRepository<AutomationRule>>(sp =>
    new EfRepository<AutomationRule>(sp.GetRequiredService<AutomationDbContext>()));
builder.Services.AddScoped<IRepository<AutomationExecutionLog>, EfRepository<AutomationExecutionLog>>(sp =>
    new EfRepository<AutomationExecutionLog>(sp.GetRequiredService<AutomationDbContext>()));

builder.Services.AddSingleton<AutomationEventProcessor>();
builder.Services.AddHostedService<EventConsumerHostedService>();

// Hangfire
var dbConn = builder.Configuration.GetConnectionString("Database");
if (!string.IsNullOrEmpty(dbConn))
{
    builder.Services.AddHangfire(config =>
        config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(dbConn)));
    builder.Services.AddHangfireServer();
}

var app = builder.Build();
app.UseOutclassInfrastructure();
app.UseMiddleware<TenantMiddleware>();
app.MapControllers();

if (!string.IsNullOrEmpty(dbConn))
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new Outclass.Automation.Infrastructure.HangfireAuthFilter() }
    });
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AutomationDbContext>();
    await db.Database.EnsureCreatedAsync();
}

Log.Information("Automation Service starting");
app.Run();
