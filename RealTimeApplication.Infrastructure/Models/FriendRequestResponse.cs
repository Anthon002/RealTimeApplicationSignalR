using RealTimeApplication.Infrastructure.Data.Utils.Enum;

namespace RealTimeApplication.Infrastructure.Models;

public sealed record FriendRequestResponse
{
    public string SenderName { get; set; } = default!;
    public string ReceiverName { get; set; } = default!;
    public FriendRequestStatusEnum Status = default!;
}
