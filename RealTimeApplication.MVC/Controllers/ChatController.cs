using Microsoft.AspNetCore.Mvc;
using MediatR;

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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}