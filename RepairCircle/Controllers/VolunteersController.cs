using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Volunteers;

namespace RepairCircle.Controllers;

public class VolunteersController : Controller
{
    private readonly IVolunteerService volunteerService;

    public VolunteersController(IVolunteerService volunteerService)
    {
        this.volunteerService = volunteerService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await volunteerService.GetApprovedAsync();
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> BecomeOne()
    {
        var model = await volunteerService.GetBecomeViewModelAsync();
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BecomeOne(VolunteerBecomeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var refreshedModel = await volunteerService.GetBecomeViewModelAsync();
            refreshedModel.Input = model.Input;
            return View(refreshedModel);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        await volunteerService.BecomeVolunteerAsync(userId, model.Input);
        return RedirectToAction(nameof(Index));
    }
}
