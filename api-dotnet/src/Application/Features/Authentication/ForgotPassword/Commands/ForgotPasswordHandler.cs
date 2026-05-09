using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using MediatR;

namespace Application.Features.Authentication;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ForgotPasswordHandler> _logger;

    public ForgotPasswordHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IDistributedCache cache,
        ILogger<ForgotPasswordHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var cacheKey = $"pwd-reset-guard:{request.Email.ToLowerInvariant()}";

        bool isGuarded = false;
        try
        {
            isGuarded = await _cache.GetStringAsync(cacheKey, cancellationToken) is not null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable — skipping idempotency guard");
        }

        if (isGuarded)
            return Result<bool>.Success(true);

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !IsUserActive(user))
            return Result<bool>.Success(true);

        try
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendPasswordReset(user.Email!, token);

            try
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    "1",
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                    },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable — idempotency guard not set");
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email for {Email}", 
                request.Email);
            return Result<bool>.Failure(Error.Internal("An error occurred processing your request."));
        }
    }

    private static bool IsUserActive(ApplicationUser user)
    {
        return user.EmailConfirmed && !user.LockoutEnabled;
    }
}