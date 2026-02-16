using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure;
using Outclass.BuildingBlocks.Infrastructure.MultiTenancy;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Workflow.Domain.Entities;
using Outclass.Workflow.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext().Enrich.WithProperty("Service", "WorkflowService")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {Message:lj}{NewLine}{Exception}"));

builder.Services.AddControllers();
builder.Services.AddOutclassInfrastructure(builder.Configuration, "workflow-service",
    typeof(Outclass.Workflow.Application.Commands.CreateWorkflowDefinitionCommand).Assembly);
builder.Services.AddServiceDbContext<WorkflowDbContext>(builder.Configuration);

builder.Services.AddScoped<IRepository<WorkflowDefinition>, EfRepository<WorkflowDefinition>>(sp =>
    new EfRepository<WorkflowDefinition>(sp.GetRequiredService<WorkflowDbContext>()));
builder.Services.AddScoped<IRepository<WorkflowInstance>, EfRepository<WorkflowInstance>>(sp =>
    new EfRepository<WorkflowInstance>(sp.GetRequiredService<WorkflowDbContext>()));

var app = builder.Build();
app.UseOutclassInfrastructure();
app.UseMiddleware<TenantMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
    await db.Database.EnsureCreatedAsync();
}

Log.Information("Workflow Service starting");
app.Run();
