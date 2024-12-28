using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace RealTimeApplication.MVC.Controllers;

public class ChatController : Controller
{
    private readonly ISender _sender;
    public ChatController(ISender sender) => _sender = sender;

    public IActionResult Index(CancellationToken cancellationToken)
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}