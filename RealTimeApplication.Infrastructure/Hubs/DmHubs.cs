using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        var sender = _context.AppUsers.FirstOrDefault(x => x.UserIdentifier == senderUserIdentifier);
        var connectionId = Context.ConnectionId;

        if (friendRequest is null)
            return;

        if (receiver is null)
            return;
        
        if (!(senderUserIdentifier == friendRequest.SenderId || senderUserIdentifier == friendRequest.ReceiverId))
            return;

        if (!(receiverUserIdentifier == friendRequest.SenderId || receiverUserIdentifier == friendRequest.ReceiverId))
            return;
        

        await Clients.Users(new List<string>{receiver.Id, sender!.Id}).SendAsync("RecieveDm", message, dmToken, connectionId);

        var msg = new Messages
        {
            ChatToken = dmToken,
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            Content = message
        };
        _context.Messages.Add(msg);
        _context.SaveChanges();
    }

    public async void NotAvailable(string recieverId, string senderId)
    {
        var reciever = _context.AppUsers.FirstOrDefault(x => x.UserIdentifier == recieverId);
        if (reciever is null)
            return;
        
        var sender = _context.AppUsers.FirstOrDefault(x => x.UserIdentifier == senderId);
        if (sender is null)
            return;

        await Clients.User(sender.Id).SendAsync("NotAvailableResponse",$"{reciever.Email} is not connected.");
    }
}
