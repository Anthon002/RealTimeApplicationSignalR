using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Models;
using RealTimeApplication.Operations.Repo.Services.Interfaces;

namespace RealTimeApplication.Operations.Handlers.Chat;

public sealed record SendInviteRequest : IRequest<BaseResponse>
{
    public string Email { get; set; } = default!;
}

public sealed class SendInviteRequestHandler : IRequestHandler<SendInviteRequest, BaseResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<SendInviteRequestHandler> _logger;
    private readonly HttpContext _httpContext;

    public SendInviteRequestHandler(AppDbContext context, IEmailService emailService, IHttpContextAccessor httpContext, ILogger<SendInviteRequestHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _httpContext = httpContext.HttpContext!;
        _logger = logger;
    }
    public async Task<BaseResponse> Handle(SendInviteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if there exists an invite object with current userid and current recepient email within the last 24 hours if yes return
            var userClaims = _httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userClaims!.Value is null || userClaims.Value == "")
                return new BaseResponse(false, "User not logged in");

            var currentUser = await _context.Users.Where(x => x.Id == userClaims.Value).Select(x => new { x.Id, x.FirstName, x.LastName }).FirstOrDefaultAsync(cancellationToken);

            var inviteExists = await _context.FriendInvites.AnyAsync(x => x.UserId == currentUser!.Id && x.RecieverEmail == request.Email.ToLower() && x.TimeCreated.Date == DateTimeOffset.UtcNow.Date, cancellationToken);

            if (inviteExists)
                return new BaseResponse(false, "You cannot send another invite to this user again for today. Please wait till tommorrow.");
            // Create the invite link
            var req = _httpContext.Request;
            var link = $"{req.Scheme}://{req.Host}/Identity/SignUp";
            // Create the invite object
            var friendInvite = new FriendInvite
            {
                RecieverEmail = request.Email.ToLower().Trim(),
                UserId = currentUser!.Id!,
                TimeCreated = DateTimeOffset.UtcNow,
                TimeUpdated = DateTimeOffset.UtcNow,
            };
            // send the invite email
            var response = await _emailService.SendEmail(new EmailRequest
            {
                HtmlContent = $"{currentUser.FirstName} {currentUser.LastName} wants you to join our platform. Sign up here <a href = '{link}'> Sign Up</a>",
                RecipientEmail = request.Email.ToLower().Trim(),
                SenderName = "Messaging App",
                SenderEmail = "",
                Subject = "Invitation to our messaging platform",
            }, cancellationToken);
            // save the invite object

            if (response.Status is false)
            {
                return response;
            }

            await _context.FriendInvites.AddAsync(friendInvite, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new BaseResponse(true, "Invite sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chat_SendInviteRequestHandler => Application ran into an error while trying to send an invite.");
            return new BaseResponse(false, "Application ran into an error while trying to send a user invite.");
        }
    }
}
