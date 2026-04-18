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

    public async Task<IActionResult> Index(string? searchTerm, string? status, int? locationId, int page = 1)
    {
        var model = await repairRequestService.GetAllAsync(searchTerm, status, locationId, page);
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> MyRequests(string? searchTerm, string? status, int? locationId, int page = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var model = await repairRequestService.GetMineAsync(userId, searchTerm, status, locationId, page);
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
        if (repairRequestId == 0)
        {
            ModelState.AddModelError(string.Empty, "The repair request could not be created. Please verify the selected location and repair session.");
            var refreshedModel = await repairRequestService.GetCreateModelAsync();
            refreshedModel.Input = model.Input;
            return View(refreshedModel);
        }

        TempData["StatusMessage"] = "Repair request submitted successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Details), new { id = repairRequestId });
    }
}
