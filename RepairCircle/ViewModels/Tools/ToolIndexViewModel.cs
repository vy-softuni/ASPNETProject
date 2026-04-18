using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.Tools;

public class ToolIndexViewModel
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? LocationId { get; set; }
    public bool? OnlyAvailable { get; set; }

    public IReadOnlyCollection<LookupViewModel> Categories { get; set; } = Array.Empty<LookupViewModel>();
    public IReadOnlyCollection<LookupViewModel> Locations { get; set; } = Array.Empty<LookupViewModel>();
    public IReadOnlyCollection<ToolListItemViewModel> Tools { get; set; } = Array.Empty<ToolListItemViewModel>();
    public PaginationViewModel Pagination { get; set; } = new();
}
