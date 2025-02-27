using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Data.Utils.Enum;

namespace RealTimeApplication.Infrastructure.Hubs;

public sealed class RealTimeDBHub : Hub
{
    private readonly AppDbContext _context;
    private readonly HttpContext _httpContext;
    public RealTimeDBHub(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContext = httpContextAccessor.HttpContext;
    }
    public void SendFriendRequests()
    {

    }
}
