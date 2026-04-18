using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;

namespace RepairCircle.Controllers;

[Authorize]
public class FavoritesController : Controller
{
    private readonly IFavoriteService favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        this.favoriteService = favoriteService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var model = await favoriteService.GetUserFavoritesAsync(userId, page);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int toolId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        await favoriteService.AddAsync(userId, toolId);
        TempData["StatusMessage"] = "Tool added to favorites.";
        TempData["StatusType"] = "success";
        return RedirectToAction("Details", "Tools", new { id = toolId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int toolId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        await favoriteService.RemoveAsync(userId, toolId);
        TempData["StatusMessage"] = "Tool removed from favorites.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }
}
