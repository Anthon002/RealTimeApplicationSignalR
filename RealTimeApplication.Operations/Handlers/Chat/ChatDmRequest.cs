using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Data.Utils.Enum;
using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Handlers.Chat;

public sealed record ChatDmRequest : IRequest<BaseResponse<RecieverEmailResponse>>
{
    public string? Token { get; set; }
}

public sealed class ChatDmRequestHandler : IRequestHandler<ChatDmRequest, BaseResponse<RecieverEmailResponse>>
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly HttpContext _httpContext;
    private readonly ILogger<ChatDmRequestHandler> _logger;

    public ChatDmRequestHandler(AppDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILogger<ChatDmRequestHandler> logger)
    {
        _context = context;
        _userManager = userManager;
        _httpContext = httpContextAccessor.HttpContext!;
        _logger = logger;
    }
    public async Task<BaseResponse<RecieverEmailResponse>> Handle(ChatDmRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var friendRequest = await _context.FriendRequests.FirstOrDefaultAsync(x => x.DmToken == request.Token, cancellationToken);

            if (friendRequest is null)
                return new BaseResponse<RecieverEmailResponse>(false, "This token does not exists.");

            if (friendRequest.Status != FriendRequestStatusEnum.Accepted)
                return new BaseResponse<RecieverEmailResponse>(false, "Friend request has not been accepted.");

            var currentUserEmail = _httpContext.User.Identity?.Name ?? "";
            var currentUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.Email == currentUserEmail, cancellationToken);

            if (currentUser is null)
                return new BaseResponse<RecieverEmailResponse>(false, "User not logged in.");

            if (!(currentUser.UserIdentifier == friendRequest.SenderId || currentUser.UserIdentifier == friendRequest.ReceiverId))
                return new BaseResponse<RecieverEmailResponse>(false, "User not verified.");

            var email1 = await _context.AppUsers.Where(x => x.UserIdentifier == friendRequest.SenderId).Select(x => new {x.Email, x.UserIdentifier, x.FirstName}).FirstOrDefaultAsync(cancellationToken);
            var email2 = await _context.AppUsers.Where(x => x.UserIdentifier == friendRequest.ReceiverId).Select(x => new {x.Email, x.UserIdentifier, x.FirstName}).FirstOrDefaultAsync(cancellationToken);

            var receipientUserId = email1!.Email == currentUserEmail ? email2!.UserIdentifier : email1.UserIdentifier;
            var friendName = email1!.Email == currentUserEmail ? email2!.FirstName : email1.FirstName;


            var response = new RecieverEmailResponse
            {
                RecieverUserId = receipientUserId,
                SenderUserId = currentUser.UserIdentifier,
                Token = friendRequest.DmToken,
                FriendName = friendName,
                CurrentUserEmail = currentUser.Email
                
            };

            return new BaseResponse<RecieverEmailResponse>(true, "User verified.", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ChatDmRequest => Application ran into an error while trying verify Chat Dm user.");
            return new BaseResponse<RecieverEmailResponse>(false, "An error occurred while trying to verify Chat Dm user.");
        }
    }
}
