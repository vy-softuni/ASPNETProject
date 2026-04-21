using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Favorites;

namespace RepairCircle.Controllers;

[Authorize]
public class FavoritesController : Controller
{
    private readonly IFavoriteService favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        this.favoriteService = favoriteService;
    }

    public async Task<IActionResult> Index(string? searchTerm, bool? onlyAvailable, int page = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        try
        {
            var model = await favoriteService.GetUserFavoritesAsync(userId, searchTerm, onlyAvailable, page);
            return View(model);
        }
        catch
        {
            TempData["StatusMessage"] = "Your favorites could not be loaded right now. Please try again later.";
            TempData["StatusType"] = "error";
            return View(new FavoriteIndexViewModel());
        }
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAjax(int toolId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { success = false, message = "You must be logged in to manage favorites." });
        }

        var result = await favoriteService.ToggleAsync(userId, toolId);
        if (result is null)
        {
            return NotFound(new { success = false, message = "Tool not found." });
        }

        return Json(new
        {
            success = true,
            isFavorited = result.IsFavorited,
            favoritesCount = result.FavoritesCount,
            message = result.Message
        });
    }
}
