using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RepairCircle.Controllers;

[Authorize]
public class BorrowRecordsController : Controller
{
    public IActionResult MyRecords() => View();
}
