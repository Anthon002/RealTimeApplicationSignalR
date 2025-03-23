using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Handlers.Identity;

public sealed record LogoutRequest : IRequest<BaseResponse<string>>
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}
public sealed class LogoutRequestHandler : IRequestHandler<LogoutRequest, BaseResponse<string>>
{
    private readonly AppDbContext _context;
    private readonly ILogger<LogoutRequestHandler> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public LogoutRequestHandler(AppDbContext context, ILogger<LogoutRequestHandler> logger, SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _logger = logger;
        _signInManager = signInManager;
    }
    public async Task<BaseResponse<string>> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _signInManager.SignOutAsync();
            return new BaseResponse<string>(true, "Logout successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application ran into an error.");
            return new BaseResponse<string>(false, "Application ran into an error.");
        }
    }
}
