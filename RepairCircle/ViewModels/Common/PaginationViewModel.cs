namespace RepairCircle.ViewModels.Common;

public class PaginationViewModel
{
    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling((double)TotalItems / PageSize);

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;

    public int StartItem => TotalItems == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;

    public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);
}
