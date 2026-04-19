using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Models;
using RepairCircle.Services.Interfaces;

namespace RepairCircle.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService homeService;

    public HomeController(IHomeService homeService)
    {
        this.homeService = homeService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await homeService.GetHomePageDataAsync();
        return View(model);
    }

    public IActionResult About() => View();

    public IActionResult Contact() => View();

    public IActionResult TestingGuide() => View();

    [Route("Home/StatusCode")]
    [ActionName("StatusCode")]
    public IActionResult HttpStatusCode(int code)
    {
        var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        var model = new StatusCodeErrorViewModel
        {
            StatusCode = code,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            OriginalPath = feature?.OriginalPath,
            OriginalQueryString = feature?.OriginalQueryString,
        };

        if (code == 404)
        {
            Response.StatusCode = 404;
            return View("NotFound", model);
        }

        Response.StatusCode = code >= 400 ? code : 500;
        return View("ServerError", model);
    }

    [Route("Home/ServerError")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [ActionName("ServerError")]
    public IActionResult ServerErrorPage()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        var model = new StatusCodeErrorViewModel
        {
            StatusCode = 500,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            OriginalPath = feature?.Path,
        };

        Response.StatusCode = 500;
        return View(model);
    }

    [Route("Home/AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
