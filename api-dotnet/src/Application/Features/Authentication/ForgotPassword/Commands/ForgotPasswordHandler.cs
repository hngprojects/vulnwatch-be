using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Scans;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordHandler> _logger;

    public ForgotPasswordHandler(UserManager<ApplicationUser> userManager, IEmailService emailService, ILogger<ForgotPasswordHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var resultModel = new Result<bool>();

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed || user.LockoutEnabled)
            {
                resultModel.AddError("User not found.", StatusCodes.Status404NotFound);
                return resultModel;
            }
            
        }
        catch (Exception ex)
        {
            
            throw;
        }


        if (user != null && await _userManager.IsEmailConfirmedAsync(user))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendPasswordReset(user.Email!, token);
        }

        return Result<string>.Success("Check your email");
    }
}