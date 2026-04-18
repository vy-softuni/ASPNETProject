namespace RepairCircle.ViewModels.Favorites;

public class FavoriteListItemViewModel
{
    public int ToolId { get; set; }
    public string ToolName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}
