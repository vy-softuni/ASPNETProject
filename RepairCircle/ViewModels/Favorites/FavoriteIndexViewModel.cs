using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.Favorites;

public class FavoriteIndexViewModel
{
    public string? SearchTerm { get; set; }
    public bool? OnlyAvailable { get; set; }

    public IReadOnlyCollection<FavoriteListItemViewModel> Items { get; set; } = Array.Empty<FavoriteListItemViewModel>();
    public PaginationViewModel Pagination { get; set; } = new();
}
