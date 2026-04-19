using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;

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
        var model = await toolService.GetAllAsync(searchTerm, categoryId, locationId, condition, onlyAvailable, page);
        return View(model);
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
