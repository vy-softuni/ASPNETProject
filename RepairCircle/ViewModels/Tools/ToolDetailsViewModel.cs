namespace RepairCircle.ViewModels.Tools;

public class ToolDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Condition { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int Quantity { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int FavoritesCount { get; set; }
    public int BorrowRecordsCount { get; set; }
    public bool IsFavoritedByCurrentUser { get; set; }
}
