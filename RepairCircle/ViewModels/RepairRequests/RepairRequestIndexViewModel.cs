using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.RepairRequests;

public class RepairRequestIndexViewModel
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public int? LocationId { get; set; }

    public IReadOnlyCollection<LookupViewModel> Locations { get; set; } = Array.Empty<LookupViewModel>();
    public IReadOnlyCollection<string> Statuses { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<RepairRequestListItemViewModel> Requests { get; set; } = Array.Empty<RepairRequestListItemViewModel>();
    public PaginationViewModel Pagination { get; set; } = new();
}
