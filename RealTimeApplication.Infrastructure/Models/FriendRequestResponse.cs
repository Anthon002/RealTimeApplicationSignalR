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
    public string? RecieverUserId { get; set; } 
    public string? SenderUserId { get; set; } 
    public string? FriendName { get; set; }
    public string? Token { get; set; } 
    public string? CurrentUserEmail { get; set; }
}

public sealed class AcceptRejectResponse
{
    public string? Token { get; set; } 
}

public sealed class FriendsListResponse
{
    public string? UserIdentifier { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Token { get; set; }
}

public sealed class UsersResponse
{
    public string? ProfilePicture { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserIdentifier { get; set; }
}
