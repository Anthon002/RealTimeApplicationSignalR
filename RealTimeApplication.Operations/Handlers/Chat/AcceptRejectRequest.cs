using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Data.Utils.Enum;
using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Handlers.Chat;
public sealed record AcceptRejectRequest : IRequest<BaseResponse<AcceptRejectResponse>>
{
    public string Id { get; set; } = default!;
    public AcceptRejectRequestEnum Purpose { get; set; }
    public string SenderUserIdentifier { get; set; } = default!;
    public string Token { get; set; } = default!;
}

public sealed class AcceptRejectRequestHandler : IRequestHandler<AcceptRejectRequest, BaseResponse<AcceptRejectResponse>>
{
    private readonly AppDbContext _context;
    private readonly ILogger<AcceptRejectRequestHandler> _logger;
    public AcceptRejectRequestHandler(AppDbContext context, ILogger<AcceptRejectRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BaseResponse<AcceptRejectResponse>> Handle(AcceptRejectRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var responseMessage = string.Empty;
            var friendRequest = await _context.FriendRequests.FirstOrDefaultAsync(x => x.ReceiverId == request.Id && x.SenderId == request.SenderUserIdentifier && x.Status == FriendRequestStatusEnum.Pending, cancellationToken);

            if (friendRequest is null)
                return new BaseResponse<AcceptRejectResponse>(false, "Friend request not found");

            if (request.Purpose == AcceptRejectRequestEnum.Accept)
            {
                friendRequest.Status = FriendRequestStatusEnum.Accepted;
                friendRequest.TimeUpdated = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return new BaseResponse<AcceptRejectResponse>(true, "Friend request has been accepted.",new AcceptRejectResponse { Token = request.Token });
            }
            else
            {
                friendRequest.Status = FriendRequestStatusEnum.Rejected;
                friendRequest.TimeUpdated = DateTimeOffset.UtcNow;
                return new BaseResponse<AcceptRejectResponse>(true, "Friend request has been rejected.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chat_AcceptRejectRequestHandler => Application ran into an error while trying to accept/reject friend request.");
            return new BaseResponse<AcceptRejectResponse>(false, "Application ran into an error while trying to accept/reject friend request.");
        }
    }
}
