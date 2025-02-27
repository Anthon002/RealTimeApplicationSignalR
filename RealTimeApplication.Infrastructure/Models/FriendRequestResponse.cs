using RealTimeApplication.Infrastructure.Data.Utils.Enum;

namespace RealTimeApplication.Infrastructure.Models;

public sealed class FriendRequestResponse
{
    public string SenderName { get; set; } = default!;
    public string ReceiverName { get; set; } = default!;
    public FriendRequestStatusEnum Status = default!;
}

public sealed class RecieverEmailResponse
{
    public string? RecieverEmail { get; set; } 
    public string? SenderEmail { get; set; } 
    public string? Token { get; set; } 
}

public sealed class AcceptRejectResponse
{
    public string? Token { get; set; } 
}
