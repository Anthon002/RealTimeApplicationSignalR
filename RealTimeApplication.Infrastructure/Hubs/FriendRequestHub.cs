using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Data.Utils.Enum;

namespace RealTimeApplication.Infrastructure.Hubs
{
    public sealed class FriendRequestHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly HttpContext _httpContext;
        public FriendRequestHub(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext.HttpContext;
        }

        public async void SendFriendRequests()
        {
            var currentUser = _httpContext.User.Identity?.Name ?? "";
            var userId = _context.AppUsers.Where(x => x.Email == currentUser).Select(x => new { x.UserIdentifier, x.Id }).FirstOrDefault();

            if (userId is null)
                return;

            var requests = from request in _context.FriendRequests.Where(x => x.ReceiverId == userId!.UserIdentifier && x.Status == FriendRequestStatusEnum.Pending)
                           from user in _context.AppUsers.Where(x => x.UserIdentifier == request.SenderId).Select(x => new { x.FirstName, x.LastName, x.UserIdentifier })
                           select new
                           {
                               FirstName = user.FirstName, //sender first name
                               LastName = user.LastName, // sender last name
                               Id = user.UserIdentifier, // sender id
                               UserId = userId.UserIdentifier, // receiver/current user id
                               Token = request.DmToken,
                           };
            var serializedResponse = JsonSerializer.Serialize(requests.ToArray());
            await Clients.User(userId.Id).SendAsync("FriendRequests", serializedResponse, "Recieved");
        }
    }
}