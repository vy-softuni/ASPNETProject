using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Enums;
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

    public async Task<ToolIndexViewModel> GetAllAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? locationId = null,
        string? condition = null,
        bool? onlyAvailable = null,
        int page = 1,
        int pageSize = 9)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 9 : pageSize;

            var toolsData = await dbContext.Tools
                .AsNoTracking()
                .Include(t => t.ToolCategory)
                .Include(t => t.Location)
                .ToListAsync();

        IEnumerable<RepairCircle.Data.Models.Tool> filteredTools = toolsData;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            filteredTools = filteredTools.Where(t =>
                t.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                t.Location.City.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                t.ToolCategory.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (categoryId.HasValue)
        {
            filteredTools = filteredTools.Where(t => t.ToolCategoryId == categoryId.Value);
        }

        if (locationId.HasValue)
        {
            filteredTools = filteredTools.Where(t => t.LocationId == locationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(condition) &&
            Enum.TryParse<ToolCondition>(condition, out var parsedCondition))
        {
            filteredTools = filteredTools.Where(t => t.Condition == parsedCondition);
        }

        if (onlyAvailable == true)
        {
            filteredTools = filteredTools.Where(t => t.IsAvailable && t.Quantity > 0);
        }

        var totalItems = filteredTools.Count();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var tools = filteredTools
            .OrderByDescending(t => t.IsAvailable && t.Quantity > 0)
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
            .ToList();

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
                Condition = condition,
                OnlyAvailable = onlyAvailable,
                Categories = categories,
                Locations = locations,
                Conditions = Enum.GetNames<ToolCondition>(),
                Tools = tools,
                Pagination = new PaginationViewModel
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalItems
                }
            };
        }
        catch
        {
            return new ToolIndexViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                LocationId = locationId,
                Condition = condition,
                OnlyAvailable = onlyAvailable,
                Conditions = Enum.GetNames<ToolCondition>(),
                Pagination = new PaginationViewModel { CurrentPage = 1, PageSize = pageSize, TotalItems = 0 }
            };
        }
    }

    public async Task<ToolDetailsViewModel?> GetByIdAsync(int id, string? currentUserId = null)
    {
        var row = await dbContext.Tools
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.ImageUrl,
                t.Condition,
                t.IsAvailable,
                t.Quantity,
                CategoryName = t.ToolCategory.Name,
                LocationName = t.Location.Name,
                Address = t.Location.Address,
                City = t.Location.City,
                Latitude = t.Location.Latitude,
                Longitude = t.Location.Longitude,
                FavoritesCount = t.Favorites.Count,
                BorrowRecordsCount = t.BorrowRecords.Count,
                IsFavoritedByCurrentUser = currentUserId != null && t.Favorites.Any(f => f.UserId == currentUserId)
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        return new ToolDetailsViewModel
        {
            Id = row.Id,
            Name = row.Name,
            Description = row.Description,
            ImageUrl = row.ImageUrl,
            Condition = row.Condition.ToString(),
            IsAvailable = row.IsAvailable,
            Quantity = row.Quantity,
            CategoryName = row.CategoryName,
            LocationName = row.LocationName,
            Address = row.Address,
            City = row.City,
            Latitude = row.Latitude,
            Longitude = row.Longitude,
            FavoritesCount = row.FavoritesCount,
            BorrowRecordsCount = row.BorrowRecordsCount,
            IsFavoritedByCurrentUser = row.IsFavoritedByCurrentUser
        };
    }
}
