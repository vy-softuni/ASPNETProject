using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class RepairSession : BaseEntity, IValidatableObject
{
    [Required(ErrorMessage = "Session title is required.")]
    [Display(Name = "Session title")]
    [StringLength(120, MinimumLength = 5, ErrorMessage = "Session title must be between 5 and 120 characters.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    [Display(Name = "Description")]
    [StringLength(1500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1500 characters.")]
    public string Description { get; set; } = null!;

    [Display(Name = "Start date and time")]
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; }

    [Display(Name = "End date and time")]
    [DataType(DataType.DateTime)]
    public DateTime EndDate { get; set; }

    [Display(Name = "Maximum participants")]
    [Range(1, 500, ErrorMessage = "Maximum participants must be between 1 and 500.")]
    public int MaxParticipants { get; set; }

    [Display(Name = "Available slots")]
    [Range(0, 500, ErrorMessage = "Available slots must be between 0 and 500.")]
    public int AvailableSlots { get; set; }

    [Display(Name = "Location")]
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public ICollection<VolunteerProfile> Volunteers { get; set; } = new HashSet<VolunteerProfile>();
    public ICollection<RepairRequest> RepairRequests { get; set; } = new HashSet<RepairRequest>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
        {
            yield return new ValidationResult(
                "End date must be after the start date.",
                new[] { nameof(EndDate) });
        }

        if (AvailableSlots > MaxParticipants)
        {
            yield return new ValidationResult(
                "Available slots cannot exceed the maximum number of participants.",
                new[] { nameof(AvailableSlots), nameof(MaxParticipants) });
        }
    }
}
