using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class Location : BaseEntity, IValidatableObject
{
    [Required(ErrorMessage = "Location name is required.")]
    [Display(Name = "Location name")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Location name must be between 3 and 100 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Address is required.")]
    [Display(Name = "Address")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters.")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "City is required.")]
    [Display(Name = "City")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "City must be between 2 and 80 characters.")]
    public string City { get; set; } = null!;

    [Display(Name = "Description")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Display(Name = "Latitude")]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public decimal? Latitude { get; set; }

    [Display(Name = "Longitude")]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public decimal? Longitude { get; set; }

    public ICollection<Tool> Tools { get; set; } = new HashSet<Tool>();
    public ICollection<RepairRequest> RepairRequests { get; set; } = new HashSet<RepairRequest>();
    public ICollection<RepairSession> RepairSessions { get; set; } = new HashSet<RepairSession>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasLatitude = Latitude.HasValue;
        var hasLongitude = Longitude.HasValue;

        if (hasLatitude != hasLongitude)
        {
            yield return new ValidationResult(
                "Please provide both latitude and longitude, or leave both empty.",
                new[] { nameof(Latitude), nameof(Longitude) });
        }
    }
}
