using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Common;
using RepairCircle.ViewModels.Favorites;

namespace RepairCircle.Services.Implementations;

public class FavoriteService : IFavoriteService
{
    private readonly ApplicationDbContext dbContext;

    public FavoriteService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<FavoriteIndexViewModel> GetUserFavoritesAsync(
        string userId,
        string? searchTerm = null,
        bool? onlyAvailable = null,
        int page = 1,
        int pageSize = 6)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 6 : pageSize;

            var favoritesData = await dbContext.Favorites
            .AsNoTracking()
            .Include(f => f.Tool)
                .ThenInclude(t => t.ToolCategory)
            .Include(f => f.Tool)
                .ThenInclude(t => t.Location)
            .Where(f => f.UserId == userId)
            .ToListAsync();

        IEnumerable<Favorite> filteredFavorites = favoritesData;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            filteredFavorites = filteredFavorites.Where(f =>
                f.Tool.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                f.Tool.ToolCategory.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                f.Tool.Location.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                f.Tool.Location.City.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (onlyAvailable == true)
        {
            filteredFavorites = filteredFavorites.Where(f => f.Tool.IsAvailable && f.Tool.Quantity > 0);
        }

        var totalItems = filteredFavorites.Count();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var favorites = filteredFavorites
            .OrderByDescending(f => f.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FavoriteListItemViewModel
            {
                ToolId = f.ToolId,
                ToolName = f.Tool.Name,
                ImageUrl = f.Tool.ImageUrl,
                CategoryName = f.Tool.ToolCategory.Name,
                LocationName = $"{f.Tool.Location.Name} ({f.Tool.Location.City})",
                IsAvailable = f.Tool.IsAvailable && f.Tool.Quantity > 0
            })
            .ToList();

            return new FavoriteIndexViewModel
            {
                SearchTerm = searchTerm,
                OnlyAvailable = onlyAvailable,
                Items = favorites,
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
            return new FavoriteIndexViewModel
            {
                SearchTerm = searchTerm,
                OnlyAvailable = onlyAvailable,
                Pagination = new PaginationViewModel { CurrentPage = 1, PageSize = pageSize, TotalItems = 0 }
            };
        }
    }

    public async Task AddAsync(string userId, int toolId)
    {
        var exists = await dbContext.Favorites.AnyAsync(f => f.UserId == userId && f.ToolId == toolId);
        var toolExists = await dbContext.Tools.AnyAsync(t => t.Id == toolId);
        if (exists || !toolExists)
        {
            return;
        }

        await dbContext.Favorites.AddAsync(new Favorite
        {
            UserId = userId,
            ToolId = toolId
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(string userId, int toolId)
    {
        var favorite = await dbContext.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ToolId == toolId);
        if (favorite is null)
        {
            return;
        }

        dbContext.Favorites.Remove(favorite);
        await dbContext.SaveChangesAsync();
    }

    public async Task<FavoriteToggleResultViewModel?> ToggleAsync(string userId, int toolId)
    {
        var tool = await dbContext.Tools
            .Include(t => t.Favorites)
            .FirstOrDefaultAsync(t => t.Id == toolId);

        if (tool is null)
        {
            return null;
        }

        var existingFavorite = tool.Favorites.FirstOrDefault(f => f.UserId == userId);
        var isFavorited = existingFavorite is null;

        if (existingFavorite is null)
        {
            tool.Favorites.Add(new Favorite
            {
                UserId = userId,
                ToolId = toolId
            });
        }
        else
        {
            dbContext.Favorites.Remove(existingFavorite);
        }

        await dbContext.SaveChangesAsync();

        return new FavoriteToggleResultViewModel
        {
            IsFavorited = isFavorited,
            FavoritesCount = tool.Favorites.Count + (isFavorited ? 1 : -1),
            Message = isFavorited ? "Tool added to favorites." : "Tool removed from favorites."
        };
    }
}
