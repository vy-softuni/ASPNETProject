using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class Skill : BaseEntity
{
    [Required]
    [StringLength(80)]
    public string Name { get; set; } = null!;

    [StringLength(300)]
    public string? Description { get; set; }

    public ICollection<VolunteerProfile> VolunteerProfiles { get; set; } = new HashSet<VolunteerProfile>();
}
