using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Data.Utils.Enum;

namespace RealTimeApplication.Infrastructure.Hubs;

public sealed class RealTimeDBHub : Hub
{
    private readonly AppDbContext _context;
    private readonly HttpContext _httpContext;
    public RealTimeDBHub(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContext = httpContextAccessor.HttpContext;
    }
    public void SendFriendRequests()
    {
        // get the id of the currently logged on user
        // use the id to get the list of friend request where reciepient Id is current user and status is pending
        // Return as serialize to json
        // arrange on frontend
        var currentUser = _httpContext.User.Identity?.Name ?? "";
        var userId = _context.AppUsers.Where(x => x.Email == currentUser).Select(x => new {x.UserIdentifier, x.Id}).FirstOrDefault();

        var requests = from request in _context.FriendRequests.Where(x => x.ReceiverId == userId!.UserIdentifier && x.Status == FriendRequestStatusEnum.Pending)
                       from user in _context.AppUsers.Where(x => x.UserIdentifier == request.SenderId).Select(x => new { x.FirstName, x.LastName, x.UserIdentifier })
                       select new
                       {
                           FirstName = user.FirstName,
                           LastName = user.LastName,
                           Id = user.UserIdentifier,
                       };
        var serializedResponse = JsonSerializer.Serialize(requests.ToArray());
        var response = Clients.User(userId!.Id).SendAsync("FriendRequests", serializedResponse, "Recieved");
    }
}
