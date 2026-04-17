using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RepairCircle.Controllers;

public class RepairRequestsController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Details(int id)
    {
        ViewBag.RequestId = id;
        return View();
    }

    [Authorize]
    public IActionResult Create() => View();
}
