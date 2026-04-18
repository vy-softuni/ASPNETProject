using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;

namespace RepairCircle.Controllers;

[Authorize]
public class BorrowRecordsController : Controller
{
    private readonly IBorrowRecordService borrowRecordService;

    public BorrowRecordsController(IBorrowRecordService borrowRecordService)
    {
        this.borrowRecordService = borrowRecordService;
    }

    public async Task<IActionResult> MyRecords()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var model = await borrowRecordService.GetUserRecordsAsync(userId);
        return View(model);
    }
}
