using RepairCircle.ViewModels.Common;
using RepairCircle.ViewModels.Favorites;

namespace RepairCircle.Services.Interfaces;

public interface IFavoriteService
{
    Task<PagedCollectionViewModel<FavoriteListItemViewModel>> GetUserFavoritesAsync(string userId, int page = 1, int pageSize = 6);
    Task AddAsync(string userId, int toolId);
    Task RemoveAsync(string userId, int toolId);
}
