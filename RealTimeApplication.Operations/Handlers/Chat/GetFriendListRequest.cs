using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Data.Utils.Enum;
using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Handlers.Chat;

public sealed record GetFriendListRequest : IRequest<BaseResponse<IEnumerable<FriendsListResponse>>>
{
    public string? Email { get; set; }
}

public sealed class GetFriendListRequestHandler : IRequestHandler<GetFriendListRequest, BaseResponse<IEnumerable<FriendsListResponse>>>
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetFriendListRequestHandler> _logger;

    public GetFriendListRequestHandler(AppDbContext context, ILogger<GetFriendListRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<BaseResponse<IEnumerable<FriendsListResponse>>> Handle(GetFriendListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Email is null)
                return new BaseResponse<IEnumerable<FriendsListResponse>>(false, "User not logged in.", new List<FriendsListResponse>{new FriendsListResponse {FirstName = "Sign up to", LastName = "get some friends"}});

            var user = await _context.AppUsers.Select(x => new { x.Email, x.UserIdentifier, x.FirstName, x.LastName }).FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
            if (user == null)
                return new BaseResponse<IEnumerable<FriendsListResponse>>(false, "User not found.", new List<FriendsListResponse>{new FriendsListResponse {FirstName = "Sign up to", LastName = "get some friends"}});

            var friends = await (from friendRequest in _context.FriendRequests.Where(x => x.Status == FriendRequestStatusEnum.Accepted && (user.UserIdentifier == x.SenderId || user.UserIdentifier == x.ReceiverId)).Select(x => new { token = x.DmToken, userId = (user.UserIdentifier == x.SenderId) ? x.ReceiverId : x.SenderId })
                                 from usr in _context.AppUsers.Where(x => x.UserIdentifier == friendRequest.userId).Select(x => new { x.FirstName, x.LastName, x.UserIdentifier })
                                 select new FriendsListResponse
                                 {
                                     FirstName = usr.FirstName,
                                     LastName = usr.LastName,
                                     UserIdentifier = usr.UserIdentifier,
                                     Token = friendRequest.token,
                                 }).ToArrayAsync(cancellationToken);
            
            if (friends is null)
            {
                friends?.Append(new FriendsListResponse
                {
                    FirstName = "Try Adding",
                    LastName =" Some Friends"
                });
            };

            return new BaseResponse<IEnumerable<FriendsListResponse>>(true, "Friend list retrieved successfully.", friends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetFriendListRequest => Application ran into an error while trying to retrieve friend list.");
            return new BaseResponse<IEnumerable<FriendsListResponse>>(false, "An error occurred while trying to verify Chat Dm user.");
        }
    }
}
