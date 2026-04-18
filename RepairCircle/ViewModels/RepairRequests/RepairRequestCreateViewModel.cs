using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.RepairRequests;

public class RepairRequestCreateViewModel
{
    public RepairRequestCreateInputModel Input { get; set; } = new();
    public IReadOnlyCollection<LookupViewModel> Locations { get; set; } = Array.Empty<LookupViewModel>();
    public IReadOnlyCollection<LookupViewModel> RepairSessions { get; set; } = Array.Empty<LookupViewModel>();
}
