using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;

namespace RealTimeApplication.Infrastructure.Hubs;
public sealed class ChatHub : Hub
{
    public long NumberOfConnections { get; set; }
    private readonly ILogger<ChatHub> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private static readonly string[] randomNames = ["Heather", "Jack", "Jane", "Olivia", "Liam", "Emma", "Noah", "Ava", "Ethan", "Mia", "Mason", "Sophia", "Logan", "Isabella", "Lucas", "Amelia", "Benjamin", "Charlotte", "Elijah", "Harper", "William", "Evelyn", "James", "Abigail", "Oliver", "Ella", "Henry", "Lily", "Alexander", "Scarlett", "Jacob", "Grace", "Michael", "Victoria", "Daniel", "Aurora", "Matthew", "Hannah", "Samuel", "Zoe", "Caleb", "Penelope", "Nathan", "Ruby", "Christopher", "Stella", "Andrew", "Aria", "Owen", "Ellie", "Ryan", "Chloe", "Dylan"];
    public ChatHub(ILogger<ChatHub> logger, IHttpContextAccessor httpContextAccessor, AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _userManager = userManager;
    }

    public async void GeneralMessage(string message)
    {
        try
        {
            int index = Convert.ToInt32(GenerateIndex(0, randomNames.Count(), Context.ConnectionId));
            var userName = randomNames[index];

            var httpContext = _httpContextAccessor.HttpContext;
            var isAuthenticated = httpContext.User.Identity?.IsAuthenticated;

            if (isAuthenticated is not null)
            {
                var boolValue = (bool)isAuthenticated;
                if (boolValue)
                {
                    userName = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                    if (userName is null)
                        userName = httpContext.User.Identity!.Name;
                }
            }
            await Clients.All.SendAsync("SendGeneralMessage", message, userName, Context.ConnectionId.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GeneralMessage => Application ran into an error.");
        }
    }

    public async void UserIsTypingNotification()
    {
        try
        {
            int index = Convert.ToInt32(GenerateIndex(0, randomNames.Count(), Context.ConnectionId));
            var userName = randomNames[index];

            var httpContext = _httpContextAccessor.HttpContext;
            var isAuthenticated = httpContext.User.Identity?.IsAuthenticated;
            if (isAuthenticated is not null)
            {
                var boolValue = (bool)isAuthenticated;
                if (boolValue)
                {
                    userName = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                    if (userName is null)
                        userName = httpContext.User.Identity!.Name;
                }
            }
            var notification = $"{userName} is Typing ...";
            await Clients.All.SendAsync("SendGeneralNotification", notification);
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

    public async void MessageToRecipient(string recipientEmail, string message)
    {
        try
        {
            if (recipientEmail == string.Empty)
                return;

            var recipient = _context.AppUsers.FirstOrDefault(x => x.Email == recipientEmail);

            if (recipient is null)
                return;

            int index = Convert.ToInt32(GenerateIndex(0, randomNames.Count(), Context.ConnectionId));
            var userName = randomNames[index];

            var httpContext = _httpContextAccessor.HttpContext;
            var isAuthenticated = httpContext.User.Identity?.IsAuthenticated;
            if (isAuthenticated is not null)
            {
                var boolValue = (bool)isAuthenticated;
                if (boolValue)
                {
                    userName = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                    if (userName is null)
                        userName = httpContext.User.Identity!.Name;
                }
            }

            await Clients.User(recipient.Id.ToString()).SendAsync("sendToRecipient", userName, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MessageToRecipient => Application ran into an error.");
        }
    }

    public async void TestHubMethod(string param1, string param2)
    {
        try
        {
            await Clients.All.SendAsync("TestMessage", param1, param2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TestHubMethod => Application ran into an error.");
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