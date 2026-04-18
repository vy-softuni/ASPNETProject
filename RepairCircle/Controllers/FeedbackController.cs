using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Feedbacks;

namespace RepairCircle.Controllers;

[Authorize]
public class FeedbackController : Controller
{
    private readonly IFeedbackService feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        this.feedbackService = feedbackService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int repairRequestId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var model = await feedbackService.GetCreateModelAsync(repairRequestId, userId);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FeedbackFormViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            model.RepairRequestTitle = model.RepairRequestTitle ?? string.Empty;
            return View(model);
        }

        var feedbackId = await feedbackService.CreateAsync(userId, model.Input);
        if (!feedbackId.HasValue)
        {
            ModelState.AddModelError(string.Empty, "Feedback could not be saved. Make sure the repair request belongs to you and you have not already left feedback.");
            var refreshedModel = await feedbackService.GetCreateModelAsync(model.Input.RepairRequestId, userId);
            if (refreshedModel is null)
            {
                return NotFound();
            }

            refreshedModel.Input = model.Input;
            return View(refreshedModel);
        }

        return RedirectToAction("Details", "RepairRequests", new { id = model.Input.RepairRequestId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var model = await feedbackService.GetEditModelAsync(id, userId);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FeedbackFormViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await feedbackService.UpdateAsync(userId, model.Input);
        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction("Details", "RepairRequests", new { id = model.Input.RepairRequestId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int repairRequestId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        await feedbackService.DeleteAsync(userId, id);
        return RedirectToAction("Details", "RepairRequests", new { id = repairRequestId });
    }
}
