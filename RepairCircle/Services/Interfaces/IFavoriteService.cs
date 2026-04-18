using RepairCircle.ViewModels.Favorites;

namespace RepairCircle.Services.Interfaces;

public interface IFavoriteService
{
    Task<FavoriteIndexViewModel> GetUserFavoritesAsync(
        string userId,
        string? searchTerm = null,
        bool? onlyAvailable = null,
        int page = 1,
        int pageSize = 6);

    Task AddAsync(string userId, int toolId);
    Task RemoveAsync(string userId, int toolId);
}
