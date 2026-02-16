using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure;
using Outclass.BuildingBlocks.Infrastructure.MultiTenancy;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Document.Domain.Entities;
using Outclass.Document.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext().Enrich.WithProperty("Service", "DocumentService")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {Message:lj}{NewLine}{Exception}"));

builder.Services.AddControllers();
builder.Services.AddOutclassInfrastructure(builder.Configuration, "document-service",
    typeof(Outclass.Document.Application.Commands.CreateDocumentCommand).Assembly);
builder.Services.AddServiceDbContext<DocumentDbContext>(builder.Configuration);
builder.Services.AddScoped<IRepository<DynamicDocument>, EfRepository<DynamicDocument>>(sp =>
    new EfRepository<DynamicDocument>(sp.GetRequiredService<DocumentDbContext>()));

var app = builder.Build();
app.UseOutclassInfrastructure();
app.UseMiddleware<TenantMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DocumentDbContext>();
    await db.Database.EnsureCreatedAsync();
}

Log.Information("Document Service starting");
app.Run();
