using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.Volunteers;

public class VolunteerBecomeViewModel
{
    public VolunteerBecomeInputModel Input { get; set; } = new();
    public IReadOnlyCollection<LookupViewModel> Skills { get; set; } = Array.Empty<LookupViewModel>();
    public IReadOnlyCollection<string> ExperienceLevels { get; set; } = Array.Empty<string>();
}
