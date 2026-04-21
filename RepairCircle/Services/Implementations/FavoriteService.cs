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
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 6 : pageSize;

        var query = dbContext.Favorites
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var likeTerm = $"%{searchTerm.Trim()}%";
            query = query.Where(f =>
                EF.Functions.Like(f.Tool.Name, likeTerm) ||
                EF.Functions.Like(f.Tool.ToolCategory.Name, likeTerm) ||
                EF.Functions.Like(f.Tool.Location.Name, likeTerm) ||
                EF.Functions.Like(f.Tool.Location.City, likeTerm));
        }

        if (onlyAvailable == true)
        {
            query = query.Where(f => f.Tool.IsAvailable && f.Tool.Quantity > 0);
        }

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var favoriteRows = await query
            .OrderByDescending(f => f.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new
            {
                f.ToolId,
                ToolName = f.Tool.Name,
                ImageUrl = f.Tool.ImageUrl,
                CategoryName = f.Tool.ToolCategory.Name,
                LocationName = f.Tool.Location.Name,
                City = f.Tool.Location.City,
                IsAvailable = f.Tool.IsAvailable && f.Tool.Quantity > 0
            })
            .ToListAsync();

        var favorites = favoriteRows
            .Select(f => new FavoriteListItemViewModel
            {
                ToolId = f.ToolId,
                ToolName = f.ToolName,
                ImageUrl = f.ImageUrl,
                CategoryName = f.CategoryName,
                LocationName = $"{f.LocationName} ({f.City})",
                IsAvailable = f.IsAvailable
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
        var toolExists = await dbContext.Tools.AnyAsync(t => t.Id == toolId);
        if (!toolExists)
        {
            return null;
        }

        var favorite = await dbContext.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ToolId == toolId);
        var isFavorited = false;
        var message = string.Empty;

        if (favorite is null)
        {
            await dbContext.Favorites.AddAsync(new Favorite
            {
                UserId = userId,
                ToolId = toolId
            });
            isFavorited = true;
            message = "Tool added to favorites.";
        }
        else
        {
            dbContext.Favorites.Remove(favorite);
            message = "Tool removed from favorites.";
        }

        await dbContext.SaveChangesAsync();

        var favoritesCount = await dbContext.Favorites.CountAsync(f => f.ToolId == toolId);

        return new FavoriteToggleResultViewModel
        {
            IsFavorited = isFavorited,
            FavoritesCount = favoritesCount,
            Message = message
        };
    }
}
