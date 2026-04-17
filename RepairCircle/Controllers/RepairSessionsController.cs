using Microsoft.AspNetCore.Mvc;

namespace RepairCircle.Controllers;

public class RepairSessionsController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Details(int id)
    {
        ViewBag.SessionId = id;
        return View();
    }
}
