namespace RepairCircle.ViewModels.Volunteers;

public class VolunteerListItemViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string ExperienceLevel { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Skills { get; set; } = Array.Empty<string>();
}
