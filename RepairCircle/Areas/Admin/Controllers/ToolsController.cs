using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Common;

namespace RepairCircle.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class ToolsController : Controller
{
    private readonly ApplicationDbContext dbContext;
    private readonly IFileStorageService fileStorageService;

    public ToolsController(ApplicationDbContext dbContext, IFileStorageService fileStorageService)
    {
        this.dbContext = dbContext;
        this.fileStorageService = fileStorageService;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int? locationId, bool? onlyAvailable, int page = 1)
    {
        const int pageSize = 10;
        page = page < 1 ? 1 : page;

        var query = dbContext.Tools
            .AsNoTracking()
            .Include(t => t.ToolCategory)
            .Include(t => t.Location)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(normalizedSearchTerm) ||
                t.Description.ToLower().Contains(normalizedSearchTerm) ||
                t.ToolCategory.Name.ToLower().Contains(normalizedSearchTerm) ||
                t.Location.Name.ToLower().Contains(normalizedSearchTerm) ||
                t.Location.City.ToLower().Contains(normalizedSearchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.ToolCategoryId == categoryId.Value);
        }

        if (locationId.HasValue)
        {
            query = query.Where(t => t.LocationId == locationId.Value);
        }

        if (onlyAvailable == true)
        {
            query = query.Where(t => t.IsAvailable && t.Quantity > 0);
        }

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var tools = await query
            .OrderBy(t => t.Name)
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
        ViewBag.CategoryId = categoryId;
        ViewBag.LocationId = locationId;
        ViewBag.OnlyAvailable = onlyAvailable;
        await PopulateLookupsAsync(categoryId, locationId);

        return View(tools);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateLookupsAsync();
        return View(new Tool { IsAvailable = true, Quantity = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tool model, IFormFile? imageFile)
    {
        if (!fileStorageService.TryValidateImage(imageFile, out var imageValidationError))
        {
            ModelState.AddModelError(nameof(model.ImageUrl), imageValidationError);
        }

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(model.ToolCategoryId, model.LocationId);
            return View(model);
        }

        if (imageFile is not null && imageFile.Length > 0)
        {
            model.ImageUrl = await fileStorageService.SaveImageAsync(imageFile, "tools");
        }

        await dbContext.Tools.AddAsync(model);
        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Tool created successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var tool = await dbContext.Tools.FindAsync(id);
        if (tool is null)
        {
            return NotFound();
        }

        await PopulateLookupsAsync(tool.ToolCategoryId, tool.LocationId);
        return View(tool);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Tool model, IFormFile? imageFile)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!fileStorageService.TryValidateImage(imageFile, out var imageValidationError))
        {
            ModelState.AddModelError(nameof(model.ImageUrl), imageValidationError);
        }

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(model.ToolCategoryId, model.LocationId);
            return View(model);
        }

        var tool = await dbContext.Tools.FindAsync(id);
        if (tool is null)
        {
            return NotFound();
        }

        tool.Name = model.Name;
        tool.Description = model.Description;
        if (imageFile is not null && imageFile.Length > 0)
        {
            fileStorageService.DeleteIfLocalUpload(tool.ImageUrl);
            tool.ImageUrl = await fileStorageService.SaveImageAsync(imageFile, "tools");
        }
        else
        {
            tool.ImageUrl = model.ImageUrl;
        }

        tool.Condition = model.Condition;
        tool.IsAvailable = model.IsAvailable;
        tool.Quantity = model.Quantity;
        tool.ToolCategoryId = model.ToolCategoryId;
        tool.LocationId = model.LocationId;
        tool.ModifiedOn = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Tool updated successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var tool = await dbContext.Tools.FindAsync(id);
        if (tool is not null)
        {
            fileStorageService.DeleteIfLocalUpload(tool.ImageUrl);
            dbContext.Tools.Remove(tool);
            await dbContext.SaveChangesAsync();
        }

        TempData["StatusMessage"] = "Tool deleted successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookupsAsync(int? selectedCategoryId = null, int? selectedLocationId = null)
    {
        ViewBag.ToolCategories = new SelectList(
            await dbContext.ToolCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
            nameof(ToolCategory.Id),
            nameof(ToolCategory.Name),
            selectedCategoryId);

        ViewBag.Locations = new SelectList(
            await dbContext.Locations.AsNoTracking().OrderBy(l => l.Name).ToListAsync(),
            nameof(Location.Id),
            nameof(Location.Name),
            selectedLocationId);
    }
}
