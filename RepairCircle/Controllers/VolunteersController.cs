using Microsoft.AspNetCore.Mvc;

namespace RepairCircle.Controllers;

public class VolunteersController : Controller
{
    public IActionResult Index() => View();

    public IActionResult BecomeOne() => View();
}
