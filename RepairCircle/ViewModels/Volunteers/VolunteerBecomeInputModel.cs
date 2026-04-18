using System.ComponentModel.DataAnnotations;

namespace RepairCircle.ViewModels.Volunteers;

public class VolunteerBecomeInputModel
{
    [StringLength(1000)]
    public string? Bio { get; set; }

    [Required]
    public string ExperienceLevel { get; set; } = string.Empty;

    public List<int> SelectedSkillIds { get; set; } = new();
}
