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
    private readonly AppDbContext _context;
    public RegistrationFormRequestHandler(ILogger<RegistrationFormRequestHandler> logger, UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
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

            var newUser = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
            };
            await _userManager.CreateAsync(newUser, request.Password);
            await _context.AppUsers.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var userClaimsPrincipal = await GenerateClaimsPrincipal(request, cancellationToken);
            if (userClaimsPrincipal is null)
            {
                return new BaseResponse<string>(false, "User not found.");
            };

            HttpContext httpContext = new DefaultHttpContext();
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userClaimsPrincipal);

            return new BaseResponse<string>(true, "User has been successfully registered.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Identity_RegistrationFormRequest => Application ran into an error while trying to sign up a new user");
            return new BaseResponse<string>(false, "Application ran into an error while trying to sign up a new user;");
        }
    }
    public async Task<ClaimsPrincipal> GenerateClaimsPrincipal(RegistrationFormRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.AppUsers.Select(x => new { x.Id, x.Email }).FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null)
            return null!;

        //var roles = await _context.Roles.Select(x => new { x.Id, x.Name }).ToArrayAsync(cancellationToken);

        // var userRoles = await _context.UserRoles.Select(x => new { x.UserId, x.RoleId }).Where(x => x.UserId == user.Id).ToArrayAsync(cancellationToken);

        var roles = await (from role in _context.Roles.Select(x => new { x.Id, x.Name })
                           from userRole in _context.UserRoles.Where(x => x.RoleId == role.Id)
                           select new
                           {
                               role.Id,
                               role.Name
                           }).ToArrayAsync(cancellationToken);

        List<Claim> userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id ),
            new Claim(ClaimTypes.Surname, request.LastName),
            new Claim(ClaimTypes.GivenName, request.FirstName),
        };
        foreach (var role in roles)
        {
            userClaims.Add(new Claim(ClaimTypes.Role, role.Name));
        }
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal newClaims = new ClaimsPrincipal(claimsIdentity);
        return newClaims;
    }
}
