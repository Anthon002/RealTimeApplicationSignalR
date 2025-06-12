namespace RealTimeApplication.Infrastructure.Models;

public sealed class MessageResponse
{
    public string SenderName { get; set; } = default!;
    public string ReceiverName { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string TimeStamp { get; set; } = default!;
}

public sealed record EmailRequest
{
    public string? SenderName { get; set; }
    public string? RecipientName { get; set; }
    public string? SenderEmail { get; set; }
    public string? RecipientEmail { get; set; }
    public string? TextContent { get; set; }
    public string? HtmlContent { get; set; }
    public string? Subject { get; set; }
};
