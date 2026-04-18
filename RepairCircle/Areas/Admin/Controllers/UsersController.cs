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

    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 10;
        page = page < 1 ? 1 : page;

        var query = userManager.Users.AsNoTracking();

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var users = await query
            .OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new List<AdminUserListItemViewModel>();
        foreach (var user in users)
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

        ViewBag.Pagination = new PaginationViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return View(model);
    }
}
