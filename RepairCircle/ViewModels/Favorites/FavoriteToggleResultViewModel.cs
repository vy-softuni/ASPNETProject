namespace RepairCircle.ViewModels.Favorites;

public class FavoriteToggleResultViewModel
{
    public bool IsFavorited { get; set; }
    public int FavoritesCount { get; set; }
    public string Message { get; set; } = string.Empty;
}
