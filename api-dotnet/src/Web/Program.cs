using Application.Interfaces;
using Application.Features.Scans;
using Infrastructure.Redis;
using StackExchange.Redis;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateScanCommand).Assembly);
});

// Redis
var redisConfig = builder.Configuration.GetValue<string>("Redis:Configuration") ?? "localhost:6379";

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(redisConfig);
    //config.AbortOnConnectFail = false; // Prevent exceptions on startup if Redis is unavailable
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddSingleton<IRedisProducer, RedisProducer>();

builder.Services.AddHealthChecks();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "docs";
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Lifetime.ApplicationStarted.Register(() =>
{
    app.Logger.LogInformation("Application has started.");
});

app.Run();
