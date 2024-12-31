using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using System.Text;


namespace RealTimeApplication.Operations.Handlers.Identity;
public sealed record RegistrationFormRequest : IRequest<BaseResponse<string>>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public sealed class RegistrationFormRequestHandler : IRequestHandler<RegistrationFormRequest, BaseResponse<string>>
{
    private readonly ILogger<RegistrationFormRequestHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContext;
    public RegistrationFormRequestHandler(ILogger<RegistrationFormRequestHandler> logger, UserManager<ApplicationUser> userManager, AppDbContext context, IHttpContextAccessor httpContext, SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
        _httpContext = httpContext;
        _signInManager = signInManager;
    }

    public async Task<BaseResponse<string>> Handle(RegistrationFormRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.FirstName is null)
            {
                return new BaseResponse<string>(false, "First Name is missing.");
            };
            if (request.LastName is null)
            {
                return new BaseResponse<string>(false, "Last Name is missing.");
            };
            if (request.Email is null)
            {
                return new BaseResponse<string>(false, "Email is missing.");
            };

            var userExists = await _context.AppUsers!.AnyAsync(x => x.Email == request.Email, cancellationToken);
            if (userExists)
                return new BaseResponse<string>(false, "This email is already taken");

            var newUser = new ApplicationUser
            {
                UserIdentifier = $"RTA{Guid.NewGuid().ToString().Substring(0, 8).Replace("-", "")}",
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            await _userManager.CreateAsync(newUser, request.Password);
            await _context.AppUsers!.AddAsync(newUser, cancellationToken);

            await _signInManager.SignInAsync(newUser, isPersistent: true);

            await _context.SaveChangesAsync(cancellationToken);

            return new BaseResponse<string>(true, "User has been successfully registered.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Identity_RegistrationFormRequest => Application ran into an error while trying to sign up a new user");
            return new BaseResponse<string>(false, "Application ran into an error while trying to sign up a new user;");
        }
    }
}
