using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace RealTimeApplication.Infrastructure.Hubs;
public sealed class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly string[] randomNames = ["Heather", "Jack", "Jane", "Olivia", "Liam", "Emma", "Noah", "Ava", "Ethan", "Mia", "Mason", "Sophia", "Logan", "Isabella", "Lucas", "Amelia", "Benjamin", "Charlotte", "Elijah", "Harper", "William", "Evelyn", "James", "Abigail", "Oliver", "Ella", "Henry", "Lily", "Alexander", "Scarlett", "Jacob", "Grace", "Michael", "Victoria", "Daniel", "Aurora", "Matthew", "Hannah", "Samuel", "Zoe", "Caleb", "Penelope", "Nathan", "Ruby", "Christopher", "Stella", "Andrew", "Aria", "Owen", "Ellie", "Ryan", "Chloe", "Dylan"];
    public ChatHub(ILogger<ChatHub> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public void GeneralMessage(string message)
    {
        try
        {
            int index = Convert.ToInt32(GenerateIndex(0, randomNames.Count(), Context.ConnectionId));
            var userName= randomNames[index];

            var httpContext = _httpContextAccessor.HttpContext;
            var isAuthenticated = httpContext.User.Identity?.IsAuthenticated;
            if (isAuthenticated is not null)
            {
                var boolValue = (bool)isAuthenticated;
                if (boolValue)
                {
                    userName = httpContext.User.Identity!.Name;
                }
            }

            Clients.All.SendAsync("SendGeneralMessage", message, userName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GeneralMessage => Application ran into an error.");
        }
    }

    public void UserIsTypingNotification()
    {
        try
        {
            int index = Convert.ToInt32(GenerateIndex(0, randomNames.Count(), Context.ConnectionId));
            var randomUserName = randomNames[index];
            var notification = $"{randomUserName} is Typing ...";
            Clients.All.SendAsync("SendGeneralNotification", notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UserIsTypingNotification => Application ran into an error.");
        }

    }

    public void UserIsNotTypingNotification()
    {
        try
        {
            Clients.All.SendAsync("SendNotTypingNotification", "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UserIsNotTypingNotification => Application ran into an error.");
        }
    }

    public double GenerateIndex(int min, int max, string ConnectionId)
    {
        using var sha256 = SHA256.Create();
        var hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(ConnectionId));

        int byteToInt = BitConverter.ToInt32(hashValue);
        double valueWithinRange = byteToInt % (max - min) - 1;
        var wholeNo = Math.Ceiling(valueWithinRange);
        wholeNo = Math.Abs(wholeNo);

        return wholeNo;
    }
}