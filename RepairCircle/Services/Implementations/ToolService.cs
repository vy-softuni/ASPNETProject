using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Common;
using RepairCircle.ViewModels.Tools;

namespace RepairCircle.Services.Implementations;

public class ToolService : IToolService
{
    private readonly ApplicationDbContext dbContext;

    public ToolService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<ToolIndexViewModel> GetAllAsync(string? searchTerm = null, int? categoryId = null, int? locationId = null, bool? onlyAvailable = null, int page = 1, int pageSize = 9)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 9 : pageSize;

        var toolsQuery = dbContext.Tools
            .AsNoTracking()
            .Include(t => t.ToolCategory)
            .Include(t => t.Location)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            toolsQuery = toolsQuery.Where(t =>
                t.Name.ToLower().Contains(normalizedSearchTerm) ||
                t.Description.ToLower().Contains(normalizedSearchTerm));
        }

        if (categoryId.HasValue)
        {
            toolsQuery = toolsQuery.Where(t => t.ToolCategoryId == categoryId.Value);
        }

        if (locationId.HasValue)
        {
            toolsQuery = toolsQuery.Where(t => t.LocationId == locationId.Value);
        }

        if (onlyAvailable == true)
        {
            toolsQuery = toolsQuery.Where(t => t.IsAvailable && t.Quantity > 0);
        }

        var totalItems = await toolsQuery.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var tools = await toolsQuery
            .OrderByDescending(t => t.IsAvailable)
            .ThenBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new ToolListItemViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ImageUrl = t.ImageUrl,
                Condition = t.Condition.ToString(),
                IsAvailable = t.IsAvailable,
                Quantity = t.Quantity,
                CategoryName = t.ToolCategory.Name,
                LocationName = t.Location.Name,
                City = t.Location.City
            })
            .ToListAsync();

        var categories = await dbContext.ToolCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new LookupViewModel
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();

        var locations = await dbContext.Locations
            .AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LookupViewModel
            {
                Id = l.Id,
                Name = $"{l.Name} ({l.City})"
            })
            .ToListAsync();

        return new ToolIndexViewModel
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            LocationId = locationId,
            OnlyAvailable = onlyAvailable,
            Categories = categories,
            Locations = locations,
            Tools = tools,
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            }
        };
    }

    public async Task<ToolDetailsViewModel?> GetByIdAsync(int id)
    {
        return await dbContext.Tools
            .AsNoTracking()
            .Include(t => t.ToolCategory)
            .Include(t => t.Location)
            .Include(t => t.Favorites)
            .Include(t => t.BorrowRecords)
            .Where(t => t.Id == id)
            .Select(t => new ToolDetailsViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ImageUrl = t.ImageUrl,
                Condition = t.Condition.ToString(),
                IsAvailable = t.IsAvailable,
                Quantity = t.Quantity,
                CategoryName = t.ToolCategory.Name,
                LocationName = t.Location.Name,
                Address = t.Location.Address,
                City = t.Location.City,
                Latitude = t.Location.Latitude,
                Longitude = t.Location.Longitude,
                FavoritesCount = t.Favorites.Count,
                BorrowRecordsCount = t.BorrowRecords.Count
            })
            .FirstOrDefaultAsync();
    }
}
