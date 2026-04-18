using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;

namespace RepairCircle.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class DashboardController : Controller
{
    private readonly IAdminDashboardService adminDashboardService;

    public DashboardController(IAdminDashboardService adminDashboardService)
    {
        this.adminDashboardService = adminDashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await adminDashboardService.GetStatisticsAsync();
        return View(model);
    }
}
