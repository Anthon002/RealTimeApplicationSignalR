using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Handlers.Identity;

public sealed record LoginFormRequest : IRequest<BaseResponse<string>>
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}
public sealed class LoginFormRequestHandler : IRequestHandler<LoginFormRequest, BaseResponse<string>>
{
    private readonly AppDbContext _context;
    private readonly ILogger<LoginFormRequestHandler> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public LoginFormRequestHandler(AppDbContext context, ILogger<LoginFormRequestHandler> logger, SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _logger = logger;
        _signInManager = signInManager;
    }
    public async Task<BaseResponse<string>> Handle(LoginFormRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.AppUsers!.FirstOrDefaultAsync(x => x.UserName == request.UserName || x.Email == request.UserName || x.PhoneNumber == request.UserName, cancellationToken);

            if (user is null)
                return new BaseResponse<string>(false, "User not found.");

            var passwordMatch = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

            if (!passwordMatch.Succeeded)
                return new BaseResponse<string>(false, "Invalid password");
            
            await _signInManager.SignInAsync(user, isPersistent: false);


            return new BaseResponse<string>(true, "Login Successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application ran into an error");
            return new BaseResponse<string>(false, "Application ran into an error");
        }
    }
}
