using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Tools;

namespace RepairCircle.Controllers;

public class ToolsController : Controller
{
    private readonly IToolService toolService;

    public ToolsController(IToolService toolService)
    {
        this.toolService = toolService;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int? locationId, string? condition, bool? onlyAvailable, int page = 1)
    {
        try
        {
            var model = await toolService.GetAllAsync(searchTerm, categoryId, locationId, condition, onlyAvailable, page);
            return View(model);
        }
        catch
        {
            TempData["StatusMessage"] = "The tool catalog could not be loaded right now. Please try again in a moment.";
            TempData["StatusType"] = "error";
            return View(new ToolIndexViewModel());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

        var model = await toolService.GetByIdAsync(id, userId);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }
}
