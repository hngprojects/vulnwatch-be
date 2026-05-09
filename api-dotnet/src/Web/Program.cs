using Application.Features.Auth;
using Application.Features.Scans;
using Web.Services;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Redis;
using Infrastructure.Services;
using Application.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;
using Web.Middleware;
using Web.Extensions;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR — scans Application assembly + Auth handlers in the same assembly
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateScanCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
});

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Default database connection string is not configured.");
}

builder.Services.AddDbContext<VulnWatchDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity — AddIdentityCore avoids overriding auth scheme to cookies
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<VulnWatchDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:SecretKey"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException("Jwt:SecretKey is not configured.");
}

var jwtKey = Encoding.UTF8.GetBytes(jwtSecret);

if (jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:SecretKey must be at least 32 characters (256 bits) for HS256 signing.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = ctx =>
            {
                ctx.HandleResponse();
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Redis
var redisConfig = builder.Configuration.GetValue<string>("Redis:Configuration") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(redisConfig);
    config.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddSingleton<IRedisProducer, RedisProducer>();

// Application services
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultHandler>();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(JwtConfig.SectionName));
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IGoogleTokenVerifier, GoogleTokenVerifier>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHealthChecks()
    .AddRedis(redisConfig, "redis");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VulnWatchDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "docs";
});

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseMiddleware<JwtMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Lifetime.ApplicationStarted.Register(() =>
{
    app.Logger.LogInformation("Application has started.");
});

app.Run();

static void LoadDotEnv()
{
    foreach (var envPath in ResolveDotEnvCandidates())
    {
        if (!File.Exists(envPath))
            continue;

        foreach (var rawLine in File.ReadAllLines(envPath))
        {
            var line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
                continue;

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();

            if (value.Length >= 2 &&
                ((value.StartsWith('"') && value.EndsWith('"')) ||
                 (value.StartsWith('\'') && value.EndsWith('\''))))
            {
                value = value[1..^1];
            }

            Environment.SetEnvironmentVariable(key, value);
        }

        return;
    }
}

static IEnumerable<string> ResolveDotEnvCandidates()
{
    var currentDirectory = Directory.GetCurrentDirectory();
    var appBaseDirectory = AppContext.BaseDirectory;

    return new[]
    {
        Path.GetFullPath(Path.Combine(currentDirectory, "api-dotnet", ".env")),
        Path.GetFullPath(Path.Combine(currentDirectory, ".env")),
        Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", ".env")),
        Path.GetFullPath(Path.Combine(appBaseDirectory, "..", "..", "..", "..", ".env")),
        Path.GetFullPath(Path.Combine(appBaseDirectory, "..", "..", "..", "..", "..", ".env"))
    }.Distinct(StringComparer.OrdinalIgnoreCase);
}
