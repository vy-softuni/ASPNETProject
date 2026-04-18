using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.Volunteers;

public class VolunteerIndexViewModel
{
    public string? SearchTerm { get; set; }
    public int? SkillId { get; set; }
    public string? ExperienceLevel { get; set; }

    public IReadOnlyCollection<LookupViewModel> Skills { get; set; } = Array.Empty<LookupViewModel>();
    public IReadOnlyCollection<string> ExperienceLevels { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<VolunteerListItemViewModel> Volunteers { get; set; } = Array.Empty<VolunteerListItemViewModel>();
    public PaginationViewModel Pagination { get; set; } = new();
}
