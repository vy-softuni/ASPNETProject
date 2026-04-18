namespace RepairCircle.ViewModels.Common;

public class PagedCollectionViewModel<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();

    public PaginationViewModel Pagination { get; set; } = new();
}
