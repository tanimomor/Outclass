using Outclass.BuildingBlocks.Domain;
using Outclass.BuildingBlocks.Infrastructure;
using Outclass.BuildingBlocks.Infrastructure.MultiTenancy;
using Outclass.BuildingBlocks.Infrastructure.Persistence;
using Outclass.Identity.Application.Services;
using Outclass.Identity.Domain.Entities;
using Outclass.Identity.Infrastructure.Persistence;
using Outclass.Identity.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Service", "IdentityService")
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {Message:lj} {Properties:j}{NewLine}{Exception}");
});

builder.Services.AddControllers();
builder.Services.AddOutclassInfrastructure(builder.Configuration, "identity-service",
    typeof(Outclass.Identity.Application.Commands.RegisterUserCommand).Assembly);

builder.Services.AddServiceDbContext<IdentityDbContext>(builder.Configuration);

// Identity-specific services
builder.Services.AddScoped<IRepository<User>, EfRepository<User>>(sp =>
    new EfRepository<User>(sp.GetRequiredService<IdentityDbContext>()));
builder.Services.AddScoped<IRepository<Role>, EfRepository<Role>>(sp =>
    new EfRepository<Role>(sp.GetRequiredService<IdentityDbContext>()));
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

var app = builder.Build();

app.UseOutclassInfrastructure();
app.UseMiddleware<TenantMiddleware>();
app.MapControllers();

// Apply migrations and seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await db.Database.EnsureCreatedAsync();
    
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await Outclass.Identity.Infrastructure.DataSeeder.SeedAsync(db, hasher);
}

Log.Information("Identity Service starting on port {Port}", builder.Configuration.GetValue<string>("ASPNETCORE_URLS") ?? "5001");
app.Run();
