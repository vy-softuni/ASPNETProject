using System.ComponentModel.DataAnnotations;
using RepairCircle.Data.Enums;

namespace RepairCircle.Data.Models;

public class RepairRequest : BaseEntity
{
    [Required]
    [StringLength(120)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = null!;

    [Required]
    [StringLength(80)]
    public string ItemType { get; set; } = null!;

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    public RepairRequestStatus Status { get; set; } = RepairRequestStatus.Submitted;

    [Required]
    public string SubmittedByUserId { get; set; } = null!;
    public ApplicationUser SubmittedByUser { get; set; } = null!;

    public int? AssignedVolunteerProfileId { get; set; }
    public VolunteerProfile? AssignedVolunteerProfile { get; set; }

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

    public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
}
