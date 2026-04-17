using Microsoft.AspNetCore.Mvc;

namespace RepairCircle.Controllers;

public class ToolsController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Details(int id)
    {
        ViewBag.ToolId = id;
        return View();
    }
}
