using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure;
using Outclass.BuildingBlocks.Infrastructure.MultiTenancy;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.FileStorage.Application.Services;
using Outclass.FileStorage.Domain.Entities;
using Outclass.FileStorage.Infrastructure.Persistence;
using Outclass.FileStorage.Infrastructure.Storage;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext().Enrich.WithProperty("Service", "FileStorageService")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {Message:lj}{NewLine}{Exception}"));

builder.Services.AddControllers();
builder.Services.AddOutclassInfrastructure(builder.Configuration, "file-service",
    typeof(Outclass.FileStorage.Application.Services.IStorageProvider).Assembly);
builder.Services.AddServiceDbContext<FileStorageDbContext>(builder.Configuration);

builder.Services.AddScoped<IRepository<FileMetadata>, EfRepository<FileMetadata>>(sp =>
    new EfRepository<FileMetadata>(sp.GetRequiredService<FileStorageDbContext>()));
builder.Services.AddSingleton<IStorageProvider>(new LocalStorageProvider());

var app = builder.Build();
app.UseOutclassInfrastructure();
app.UseMiddleware<TenantMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FileStorageDbContext>();
    await db.Database.EnsureCreatedAsync();
}

Log.Information("File Storage Service starting");
app.Run();
