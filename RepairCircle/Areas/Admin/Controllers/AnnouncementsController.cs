using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;
using RepairCircle.ViewModels.Common;

namespace RepairCircle.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class AnnouncementsController : Controller
{
    private readonly ApplicationDbContext dbContext;

    public AnnouncementsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IActionResult> Index(string? searchTerm, bool? onlyImportant, bool? onlyPublished, int page = 1)
    {
        const int pageSize = 10;
        page = page < 1 ? 1 : page;

        var query = dbContext.Announcements.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(a =>
                a.Title.ToLower().Contains(normalizedSearchTerm) ||
                a.Content.ToLower().Contains(normalizedSearchTerm));
        }

        if (onlyImportant == true)
        {
            query = query.Where(a => a.IsImportant);
        }

        if (onlyPublished == true)
        {
            query = query.Where(a => a.IsPublished);
        }

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var announcements = await query
            .OrderByDescending(a => a.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Pagination = new PaginationViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
        ViewBag.SearchTerm = searchTerm;
        ViewBag.OnlyImportant = onlyImportant;
        ViewBag.OnlyPublished = onlyPublished;

        return View(announcements);
    }

    [HttpGet]
    public IActionResult Create() => View(new Announcement { IsPublished = true });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Announcement model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await dbContext.Announcements.AddAsync(model);
        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Announcement created successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var announcement = await dbContext.Announcements.FindAsync(id);
        if (announcement is null)
        {
            return NotFound();
        }

        return View(announcement);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Announcement model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var announcement = await dbContext.Announcements.FindAsync(id);
        if (announcement is null)
        {
            return NotFound();
        }

        announcement.Title = model.Title;
        announcement.Content = model.Content;
        announcement.IsImportant = model.IsImportant;
        announcement.IsPublished = model.IsPublished;
        announcement.ModifiedOn = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Announcement updated successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var announcement = await dbContext.Announcements.FindAsync(id);
        if (announcement is not null)
        {
            dbContext.Announcements.Remove(announcement);
            await dbContext.SaveChangesAsync();
        }

        TempData["StatusMessage"] = "Announcement deleted successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }
}
