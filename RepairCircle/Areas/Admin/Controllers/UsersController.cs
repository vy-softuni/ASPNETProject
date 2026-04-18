using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data.Models;
using RepairCircle.ViewModels.Admin;
using RepairCircle.ViewModels.Common;

namespace RepairCircle.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? role, int page = 1)
    {
        const int pageSize = 10;
        page = page < 1 ? 1 : page;

        var allUsers = await userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.Email)
            .ToListAsync();

        var model = new List<AdminUserListItemViewModel>();
        foreach (var user in allUsers)
        {
            var roles = await userManager.GetRolesAsync(user);
            model.Add(new AdminUserListItemViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName,
                CreatedOn = user.CreatedOn,
                Roles = roles.OrderBy(r => r).ToList()
            });
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            model = model.Where(u =>
                    (!string.IsNullOrWhiteSpace(u.FullName) && u.FullName.ToLower().Contains(normalizedSearchTerm)) ||
                    (!string.IsNullOrWhiteSpace(u.UserName) && u.UserName.ToLower().Contains(normalizedSearchTerm)) ||
                    u.Email.ToLower().Contains(normalizedSearchTerm))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            model = model.Where(u => u.Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        var totalItems = model.Count;
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var pagedUsers = model
            .OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.Pagination = new PaginationViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
        ViewBag.SearchTerm = searchTerm;
        ViewBag.Role = role;
        ViewBag.Roles = new[] { "Administrator", "Volunteer", "User" };

        return View(pagedUsers);
    }
}
