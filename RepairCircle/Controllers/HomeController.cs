using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Models;

namespace RepairCircle.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult About() => View();

    public IActionResult Contact() => View();

    public IActionResult StatusCode(int code)
    {
        if (code == 404)
        {
            return View("NotFound");
        }

        return View("ServerError");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
