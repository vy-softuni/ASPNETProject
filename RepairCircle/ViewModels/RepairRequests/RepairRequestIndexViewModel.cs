using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.RepairRequests;

public class RepairRequestIndexViewModel
{
    public IReadOnlyCollection<RepairRequestListItemViewModel> Requests { get; set; } = Array.Empty<RepairRequestListItemViewModel>();

    public PaginationViewModel Pagination { get; set; } = new();
}
