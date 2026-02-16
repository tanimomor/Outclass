using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext().Enrich.WithProperty("Service", "Gateway")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {Message:lj}{NewLine}{Exception}"));

// YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// JWT Authentication
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Secret") ?? "OutclassPlatformSuperSecretKey2024!@#$%^&*()_+";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer") ?? "outclass",
            ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience") ?? "outclass-api",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault() ?? "anonymous";
        return RateLimitPartition.GetFixedWindowLimiter(tenantId, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 10,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });

    options.RejectionStatusCode = 429;
});

// Health Checks
builder.Services.AddHealthChecks();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetValue<string>("Frontend:Url") ?? "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Correlation ID middleware
app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
    context.Response.Headers["X-Correlation-Id"] = correlationId;
    context.Items["CorrelationId"] = correlationId;
    await next();
});

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapReverseProxy();

Log.Information("API Gateway starting");
app.Run();
