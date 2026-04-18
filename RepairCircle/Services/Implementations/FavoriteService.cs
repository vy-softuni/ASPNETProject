using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Favorites;

namespace RepairCircle.Services.Implementations;

public class FavoriteService : IFavoriteService
{
    private readonly ApplicationDbContext dbContext;

    public FavoriteService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<FavoriteListItemViewModel>> GetUserFavoritesAsync(string userId)
    {
        return await dbContext.Favorites
            .AsNoTracking()
            .Include(f => f.Tool)
                .ThenInclude(t => t.ToolCategory)
            .Include(f => f.Tool)
                .ThenInclude(t => t.Location)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedOn)
            .Select(f => new FavoriteListItemViewModel
            {
                ToolId = f.ToolId,
                ToolName = f.Tool.Name,
                ImageUrl = f.Tool.ImageUrl,
                CategoryName = f.Tool.ToolCategory.Name,
                LocationName = $"{f.Tool.Location.Name} ({f.Tool.Location.City})",
                IsAvailable = f.Tool.IsAvailable && f.Tool.Quantity > 0
            })
            .ToListAsync();
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
}
