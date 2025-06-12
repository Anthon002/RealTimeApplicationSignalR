using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeApplication.Infrastructure.Data.Entities;
using Microsoft.Extensions.DependencyInjection;


namespace RealTimeApplication.Infrastructure.Hubs;

public sealed class DmHubs : Hub
{
    private readonly AppDbContext _context;

    public DmHubs(AppDbContext context)
    {
        _context = context;
    }

    public async Task SendDirectMessge(string receiverUserIdentifier, string senderUserIdentifier, string dmToken, string message)
    {
        var connectionId = Context.ConnectionId;
        var friendRequest = await _context.FriendRequests.FirstOrDefaultAsync(x => x.DmToken == dmToken);
        var receiver = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserIdentifier == receiverUserIdentifier);
        var sender = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserIdentifier == senderUserIdentifier);

        if (friendRequest is null)
            return;

        if (receiver is null)
            return;

        if (!(senderUserIdentifier == friendRequest.SenderId || senderUserIdentifier == friendRequest.ReceiverId))
            return;

        if (!(receiverUserIdentifier == friendRequest.SenderId || receiverUserIdentifier == friendRequest.ReceiverId))
            return; 

        await Clients.Users(new List<string> { receiver.Id, sender!.Id }).SendAsync("RecieveDm", message, dmToken, connectionId, sender.Id, receiver.Id);

        var msg = new Messages
        {
            ChatToken = dmToken,
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            TimeCreated = DateTimeOffset.UtcNow,
            TimeUpdated = DateTimeOffset.UtcNow,
            Content = message
        };
        await _context.Messages.AddAsync(msg);
        await _context.SaveChangesAsync();
    }

    public async void NotAvailable(string recieverId, string senderId)
    {
        var reciever = _context.AppUsers.FirstOrDefault(x => x.UserIdentifier == recieverId);
        if (reciever is null)
            return;

        var sender = _context.AppUsers.FirstOrDefault(x => x.UserIdentifier == senderId);
        if (sender is null)
            return;

        await Clients.User(sender.Id).SendAsync("NotAvailableResponse", $"{reciever.Email} is not connected.");
    }
}
