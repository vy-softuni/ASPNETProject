using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;

namespace RepairCircle.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class ToolsController : Controller
{
    private readonly ApplicationDbContext dbContext;

    public ToolsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var tools = await dbContext.Tools
            .AsNoTracking()
            .Include(t => t.ToolCategory)
            .Include(t => t.Location)
            .OrderBy(t => t.Name)
            .ToListAsync();

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
    public async Task<IActionResult> Create(Tool model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(model.ToolCategoryId, model.LocationId);
            return View(model);
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
    public async Task<IActionResult> Edit(int id, Tool model)
    {
        if (id != model.Id)
        {
            return BadRequest();
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
        tool.ImageUrl = model.ImageUrl;
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
