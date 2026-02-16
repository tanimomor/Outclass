using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure;
using Outclass.BuildingBlocks.Infrastructure.MultiTenancy;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Metadata.Domain.Entities;
using Outclass.Metadata.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext().Enrich.WithProperty("Service", "MetadataService")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {Message:lj}{NewLine}{Exception}"));

builder.Services.AddControllers();
builder.Services.AddOutclassInfrastructure(builder.Configuration, "metadata-service",
    typeof(Outclass.Metadata.Application.Commands.CreateEntityDefinitionCommand).Assembly);
builder.Services.AddServiceDbContext<MetadataDbContext>(builder.Configuration);
builder.Services.AddScoped<IRepository<EntityDefinition>, EfRepository<EntityDefinition>>(sp =>
    new EfRepository<EntityDefinition>(sp.GetRequiredService<MetadataDbContext>()));

var app = builder.Build();
app.UseOutclassInfrastructure();
app.UseMiddleware<TenantMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MetadataDbContext>();
    await db.Database.EnsureCreatedAsync();
}

Log.Information("Metadata Service starting");
app.Run();
