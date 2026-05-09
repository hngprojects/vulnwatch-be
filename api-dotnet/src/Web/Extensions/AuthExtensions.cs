using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Helpers;
using Application.Interfaces;
using Infrastructure.Services;
using Domain.Common;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Net;
using System.Text.Json;

namespace Web.Extensions;

public class AuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
 
    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // Let successful requests through immediately
        if (authorizeResult.Succeeded)
        {
            await next(context);
            return;
        }
 
        // Pick the right error based on whether the user is authenticated at all
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
 
        Error error;
        HttpStatusCode statusCode;
 
        if (!isAuthenticated)
        {
            error      = context.Items.TryGetValue("AuthError", out var stored) && stored is Error authErr
                         ? authErr
                         : Error.Unauthorized("No valid token");
            statusCode = HttpStatusCode.Unauthorized;
        }
        else
        {
            error      = Error.Forbidden("Insufficient permission.");
            statusCode = HttpStatusCode.Forbidden;
        }
 
        await WriteErrorResponseAsync(context, error, statusCode);
    }
 
    private static async Task WriteErrorResponseAsync(HttpContext context, Error error, HttpStatusCode statusCode)
    {
        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/json";
 
        var body = JsonSerializer.Serialize(new
        {
            code    = error.Code,
            message = error.Message
        });
 
        await context.Response.WriteAsync(body);
    }
}

public static class AuthExtensions
{
    public static IServiceCollection AddVulnWatchAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));

        var jwtOptions = configuration
            .GetSection(JwtConfig.SectionName)
            .Get<JwtConfig>()
            ?? throw new InvalidOperationException("JWT configuration is missing from appsettings.");

        services.AddSingleton<IJwtService, JwtService>();
        services.AddScoped<ISessionService, SessionService>();

        services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultHandler>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidIssuer              = jwtOptions.Issuer,
                    ValidateAudience         = true,
                    ValidAudience            = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(
                                                  Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.FromSeconds(30)
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
        
        services.AddAuthorization();

        return services;

    }
}