using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class RepairSession : BaseEntity
{
    [Required]
    [StringLength(120)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(1500)]
    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [Range(1, 500)]
    public int MaxParticipants { get; set; }

    [Range(0, 500)]
    public int AvailableSlots { get; set; }

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public ICollection<VolunteerProfile> Volunteers { get; set; } = new HashSet<VolunteerProfile>();
    public ICollection<RepairRequest> RepairRequests { get; set; } = new HashSet<RepairRequest>();
}
