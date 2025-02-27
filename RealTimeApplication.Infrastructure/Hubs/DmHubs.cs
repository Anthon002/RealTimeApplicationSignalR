using Microsoft.AspNetCore.SignalR;
using RealTimeApplication.Infrastructure.Data.Entities;

namespace RealTimeApplication.Infrastructure.Hubs;

public sealed class DmHubs : Hub
{
    private readonly AppDbContext _context;

    public DmHubs(AppDbContext context)
    {
        _context = context;
    }

    public async void SendDirectMessge(string receiverUserIdentifier, string senderUserIdentifier, string dmToken, string message)
    {
        var friendRequest = _context.FriendRequests.FirstOrDefault(x => x.DmToken == dmToken);
        var receiver = _context.AppUsers.FirstOrDefault(x => x.UserIdentifier == receiverUserIdentifier);

        if (friendRequest is null)
            return;

        if (receiver is null)
            return;

        if (senderUserIdentifier != friendRequest.SenderId || senderUserIdentifier != friendRequest.ReceiverId)
            return;

        if (receiverUserIdentifier != friendRequest.SenderId || receiverUserIdentifier != friendRequest.ReceiverId)
            return;

        await Clients.User(receiver.Id).SendAsync(message);
    }
}
