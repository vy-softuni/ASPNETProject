using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.RepairSessions;

public class RepairSessionIndexViewModel
{
    public string? SearchTerm { get; set; }
    public int? LocationId { get; set; }

    public IReadOnlyCollection<LookupViewModel> Locations { get; set; } = Array.Empty<LookupViewModel>();
    public IReadOnlyCollection<RepairSessionListItemViewModel> Sessions { get; set; } = Array.Empty<RepairSessionListItemViewModel>();
    public PaginationViewModel Pagination { get; set; } = new();
}
