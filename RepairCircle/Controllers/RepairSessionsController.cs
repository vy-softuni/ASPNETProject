using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;

namespace RepairCircle.Controllers;

public class RepairSessionsController : Controller
{
    private readonly IRepairSessionService repairSessionService;

    public RepairSessionsController(IRepairSessionService repairSessionService)
    {
        this.repairSessionService = repairSessionService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await repairSessionService.GetAllUpcomingAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await repairSessionService.GetByIdAsync(id);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }
}
