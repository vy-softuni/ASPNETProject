using RepairCircle.ViewModels.Favorites;

namespace RepairCircle.Services.Interfaces;

public interface IFavoriteService
{
    Task<IReadOnlyCollection<FavoriteListItemViewModel>> GetUserFavoritesAsync(string userId);
    Task AddAsync(string userId, int toolId);
    Task RemoveAsync(string userId, int toolId);
}
