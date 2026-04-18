using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.BorrowRecords;

namespace RepairCircle.Controllers;

[Authorize]
public class BorrowRecordsController : Controller
{
    private readonly IBorrowRecordService borrowRecordService;

    public BorrowRecordsController(IBorrowRecordService borrowRecordService)
    {
        this.borrowRecordService = borrowRecordService;
    }

    public async Task<IActionResult> MyRecords(string? searchTerm, string? status, int page = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var model = await borrowRecordService.GetUserRecordsAsync(userId, searchTerm, status, page);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int toolId)
    {
        var model = await borrowRecordService.GetCreateModelAsync(toolId);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BorrowRecordCreateViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            var refreshedModel = await borrowRecordService.GetCreateModelAsync(model.Input.ToolId);
            if (refreshedModel is null)
            {
                return NotFound();
            }

            refreshedModel.Input = model.Input;
            return View(refreshedModel);
        }

        var borrowRecordId = await borrowRecordService.CreateAsync(userId, model.Input);
        if (borrowRecordId == 0)
        {
            ModelState.AddModelError(string.Empty, "This tool is currently unavailable for borrowing.");
            var refreshedModel = await borrowRecordService.GetCreateModelAsync(model.Input.ToolId);
            if (refreshedModel is null)
            {
                return NotFound();
            }

            refreshedModel.Input = model.Input;
            return View(refreshedModel);
        }

        TempData["StatusMessage"] = "Borrowing request submitted successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(MyRecords));
    }
}
