using System.ComponentModel.DataAnnotations;
using RepairCircle.Data.Enums;

namespace RepairCircle.Data.Models;

public class RepairRequest : BaseEntity
{
    [Required(ErrorMessage = "Repair request title is required.")]
    [Display(Name = "Request title")]
    [StringLength(120, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 120 characters.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    [Display(Name = "Problem description")]
    [StringLength(2000, MinimumLength = 15, ErrorMessage = "Description must be between 15 and 2000 characters.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Item type is required.")]
    [Display(Name = "Item type")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "Item type must be between 3 and 80 characters.")]
    public string ItemType { get; set; } = null!;

    [Display(Name = "Image URL")]
    [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters.")]
    [Url(ErrorMessage = "Please enter a valid image URL.")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Status")]
    public RepairRequestStatus Status { get; set; } = RepairRequestStatus.Submitted;

    [Required]
    public string SubmittedByUserId { get; set; } = null!;
    public ApplicationUser SubmittedByUser { get; set; } = null!;

    [Display(Name = "Assigned volunteer")]
    public int? AssignedVolunteerProfileId { get; set; }
    public VolunteerProfile? AssignedVolunteerProfile { get; set; }

    [Display(Name = "Location")]
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    [Display(Name = "Repair session")]
    public int? RepairSessionId { get; set; }
    public RepairSession? RepairSession { get; set; }

    [Display(Name = "Requested date")]
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

    public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
}
