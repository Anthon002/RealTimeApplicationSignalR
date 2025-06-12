using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Net;
using RealTimeApplication.Infrastructure.Models;
using RealTimeApplication.Operations.Handlers.Chat;
using Microsoft.AspNetCore.Authorization;

namespace RealTimeApplication.MVC.Controllers;
public class ChatController : Controller
{
    private readonly ISender _sender;
    private readonly IHttpContextAccessor _httpContext;
    public ChatController(ISender sender, IHttpContextAccessor httpContext)
    {
        _sender = sender;
        _httpContext = httpContext;
    }

    public async Task<IActionResult> Index(string? responseMessage = default ,CancellationToken cancellationToken = default!)
    {
        var httpContext = _httpContext.HttpContext;
        var user = httpContext?.User.Identity;
        ViewData["user"] = user?.Name;
        TempData["InviteResponseMessage"] = responseMessage;
        var friendList = await _sender.Send(new GetFriendListRequest { Email = user?.Name }, cancellationToken);

        return View(friendList.Value);
    }

    [HttpGet("Users")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BaseResponse<PaginatedData<UsersResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);
        if (!response.Status)
            RedirectToAction("Index", new { errorMessage = response.Message });
        return Ok(response);
    }

    [HttpGet("GetUsersList")]
    [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
    public IActionResult GetUsersList(CancellationToken cancellationToken)
    {
        return View();
    }

    [HttpPost("FriendRequest")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _sender.Send(request, cancellationToken);
        return View(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public IActionResult GetFriendRequests(CancellationToken cancellationToken)
    {
        return View();
    }

    [HttpPost("AcceptReject")]
    [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AcceptReject([FromQuery] string id, [FromBody] AcceptRejectRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var response = await _sender.Send(request, cancellationToken);
        if (response.Value != null)
            return RedirectToAction("ChatDm", new { token = response.Value.Token });
        return RedirectToAction("GetFriendRequest");
    }

    [HttpGet("ChatDm/{token}")]
    [ProducesResponseType(typeof(BaseResponse<RecieverEmailResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChatDm([FromRoute] string token, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new ChatDmRequest { Token = token }, cancellationToken);
        var userName = _httpContext.HttpContext?.User.Identity?.Name ?? "";
        var friendListResponse = await _sender.Send(new GetFriendListRequest { Email = userName }, cancellationToken);
        var friends = friendListResponse.Value?.ToList();
        var currentFriend = friends?.FirstOrDefault(x => x.Token == token);
        friends?.Remove(currentFriend!);
        ViewData["FriendsList"] = friends ?? new List<FriendsListResponse>{ new FriendsListResponse {FirstName = "Get More Friends"} };
        ViewData["User"] = userName;
        if (!response.Status)
            return RedirectToAction("AcceptRejectRequest", "Chat");
        return View(response.Value);
    }

    [HttpGet("Friends")]
    [ProducesResponseType(typeof(BaseResponse<List<FriendsListResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetFriendList(CancellationToken cancellationToken)
    {
        var userName = _httpContext.HttpContext?.User.Identity?.Name;
        var response = await _sender.Send(new GetFriendListRequest { Email = userName }, cancellationToken);
        if (!response.Status)
            return RedirectToAction("Login", "Identity", new { errormessage = response.Message });
        return View(response.Value);
    }

    [HttpGet("GetFriends")]
    [ProducesResponseType(typeof(BaseResponse<List<FriendsListResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetFriendsList(CancellationToken cancellationToken)
    {
        var userName = _httpContext.HttpContext?.User.Identity?.Name;
        var response = await _sender.Send(new GetFriendListRequest { Email = userName }, cancellationToken);
        if (!response.Status)
            return RedirectToAction("Login", "Identity", new { errormessage = response.Message });
        return Ok(response);
    }

    [HttpGet("{token}/Messages")]
    [ProducesResponseType(typeof(BaseResponse<PaginatedData<MessageResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetMessages([FromRoute] string token, [FromQuery] GetMessagesRequest request, CancellationToken cancellationToken)
    {
        request.Token = token;
        var response = await _sender.Send(request, cancellationToken);
        if (!response.Status)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpPost("Invite")]
    [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
    [Authorize]
    public async Task<IActionResult> SendInvites([FromForm] SendInviteRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);
        ViewData["InviteResponseMessage"] = response.Message;
        return RedirectToAction("Index","Chat", new { responseMessage = response.Message});
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}