using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Net;
using RealTimeApplication.Infrastructure.Models;
using RealTimeApplication.Operations.Handlers.Chat;
using System.Threading.Tasks;

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

    public IActionResult Index(CancellationToken cancellationToken)
    {
        var httpContext = _httpContext.HttpContext;
        var user = httpContext?.User.Identity;
        return View();
    }

    [HttpPost("{id:long}/FriendRequest")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SendFriendRequest([FromForm] SendFriendRequest request, CancellationToken cancellationToken = default)
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
    [Produces("applicaiton/json")]
    [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AcceptReject([FromRoute] string id, [FromBody] AcceptRejectRequest request, CancellationToken cancellationToken)
    {
       // request.Id = id;
        var response = await _sender.Send(request, cancellationToken);
        if (response.Value != null)
            return RedirectToAction("ChatDm",new {token = response.Value.Token});
        return RedirectToAction("GetFriendRequest");
    }
    
    [HttpGet("ChatDm")]
    [ProducesResponseType(typeof(BaseResponse<RecieverEmailResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChatDm([FromRoute] string token, CancellationToken cancellationToken)
    {
        var response = await _sender.Send( new ChatDmRequest {Token = token}, cancellationToken);
        if (response.Status)
            return RedirectToAction("AcceptRejectRequest","Chat");
        return View(response.Value);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}