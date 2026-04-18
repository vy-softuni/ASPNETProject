using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.RepairRequests;

namespace RepairCircle.Controllers;

public class RepairRequestsController : Controller
{
    private readonly IRepairRequestService repairRequestService;

    public RepairRequestsController(IRepairRequestService repairRequestService)
    {
        this.repairRequestService = repairRequestService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await repairRequestService.GetAllAsync();
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> MyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var model = await repairRequestService.GetMineAsync(userId);
        ViewData["Title"] = "My Repair Requests";
        return View("Index", model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await repairRequestService.GetByIdAsync(id);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = await repairRequestService.GetCreateModelAsync();
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairRequestCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var refreshedModel = await repairRequestService.GetCreateModelAsync();
            refreshedModel.Input = model.Input;
            return View(refreshedModel);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var repairRequestId = await repairRequestService.CreateAsync(model.Input, userId);
        return RedirectToAction(nameof(Details), new { id = repairRequestId });
    }
}
