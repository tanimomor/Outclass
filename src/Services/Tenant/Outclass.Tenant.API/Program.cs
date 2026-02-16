using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure;
using Outclass.BuildingBlocks.Infrastructure.MultiTenancy;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Tenant.Domain.Entities;
using Outclass.Tenant.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "TenantService")
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {Message:lj}{NewLine}{Exception}");
});

builder.Services.AddControllers();
builder.Services.AddOutclassInfrastructure(builder.Configuration, "tenant-service",
    typeof(Outclass.Tenant.Application.Commands.ProvisionTenantCommand).Assembly);

builder.Services.AddServiceDbContext<TenantDbContext>(builder.Configuration);
builder.Services.AddScoped<IRepository<TenantEntity>, EfRepository<TenantEntity>>(sp =>
    new EfRepository<TenantEntity>(sp.GetRequiredService<TenantDbContext>()));

var app = builder.Build();

app.UseOutclassInfrastructure();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    await db.Database.EnsureCreatedAsync();
}

Log.Information("Tenant Service starting");
app.Run();
