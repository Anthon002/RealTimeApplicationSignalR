using System.ComponentModel.DataAnnotations;
using RealTimeApplication.Infrastructure.Data.Utils.Enum;

namespace RealTimeApplication.Infrastructure.Data.Entities;

public sealed class FriendRequests : BaseEntity
{
    [MaxLength(25)]
    public string SenderId { get; set; } = default!;
    [MaxLength(25)]
    public string ReceiverId { get; set; } = default!;
    public FriendRequestStatusEnum Status { get; set; }
    public string? DmToken { get; set; }
    
}
