using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Data.Utils.Enum;
using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Handlers.Chat;

public sealed record SendFriendRequest : IRequest<BaseResponse<string>>
{
    public string? Email { get; set; }
}

public sealed class SendFriendRequestHandler : IRequestHandler<SendFriendRequest, BaseResponse<string>>
{
    private readonly AppDbContext _context;
    private readonly ILogger<SendFriendRequestHandler> _logger;
    private readonly HttpContext _httpContext;
    public SendFriendRequestHandler(AppDbContext context, ILogger<SendFriendRequestHandler> logger, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _httpContext = httpContextAccessor.HttpContext!;
    }
    public async Task<BaseResponse<string>> Handle(SendFriendRequest request, CancellationToken cancellationToken)
    {
        try
        {
        var recipient = await _context.Users.Select(x => new { x.Email, x.UserIdentifier }).FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        var userId = _httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        if (userId is null)
            return new BaseResponse<string>(false, "You are not signed in.");
        if (recipient is null)
            return new BaseResponse<string>(false, "This user does not exists.");

        var user = await _context.Users.Select(x => new {x.UserIdentifier, x.Id}).FirstOrDefaultAsync(x => x.Id == userId.Value);

        //Implement email notification

        var friendRequest = new FriendRequests
        {
            ReceiverId = recipient.UserIdentifier,
            SenderId = user!.UserIdentifier,
            Status = FriendRequestStatusEnum.Pending,
            TimeCreated = DateTimeOffset.UtcNow,
            TimeUpdated = DateTimeOffset.UtcNow,
        };
        await _context.FriendRequests.AddAsync(friendRequest, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new BaseResponse<string>(true, "Friend request sent successfully.");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Chat_SendFriendRequestHandler => Application ran into an error while trying to send friend request.");
            return new BaseResponse<string>(false, "Application ran into an error while trying to send friend request.");
        }


    }
}
