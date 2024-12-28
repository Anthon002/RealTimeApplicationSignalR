using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealTimeApplication.Infrastructure.Models;
using RealTimeApplication.Operations.Handlers.Identity;

namespace RealTimeApplication.MVC.Controllers;

[Tags("Identity")]
public class IdentityController : Controller
{
    private readonly ISender _sender;

    public IdentityController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<string>),(int)HttpStatusCode.OK)]
    public IActionResult SignUp([FromForm]RegistrationFormRequest request, CancellationToken cancellationToken)
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}