using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.RepairSessions;

public class RepairSessionIndexViewModel
{
    public IReadOnlyCollection<RepairSessionListItemViewModel> Sessions { get; set; } = Array.Empty<RepairSessionListItemViewModel>();

    public PaginationViewModel Pagination { get; set; } = new();
}
