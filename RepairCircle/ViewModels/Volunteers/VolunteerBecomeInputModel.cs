using System.ComponentModel.DataAnnotations;

namespace RepairCircle.ViewModels.Volunteers;

public class VolunteerBecomeInputModel : IValidatableObject
{
    [Display(Name = "Short bio")]
    [StringLength(1000, MinimumLength = 20, ErrorMessage = "The bio must be between 20 and 1000 characters.")]
    public string? Bio { get; set; }

    [Required(ErrorMessage = "Please select your experience level.")]
    [Display(Name = "Experience level")]
    public string ExperienceLevel { get; set; } = string.Empty;

    [Display(Name = "Skills")]
    public List<int> SelectedSkillIds { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SelectedSkillIds.Count == 0)
        {
            yield return new ValidationResult(
                "Please select at least one skill.",
                new[] { nameof(SelectedSkillIds) });
        }
    }
}
