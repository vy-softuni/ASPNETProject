using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RepairCircle.Data.Enums;

namespace RepairCircle.Data.Models;

public class VolunteerProfile : BaseEntity
{
    [Required]
    public string UserId { get; set; } = null!;
    [ValidateNever]
    public ApplicationUser User { get; set; } = null!;

    [StringLength(1000)]
    public string? Bio { get; set; }

    public ExperienceLevel ExperienceLevel { get; set; } = ExperienceLevel.Beginner;

    public bool IsApproved { get; set; }

    [ValidateNever]
    public ICollection<Skill> Skills { get; set; } = new HashSet<Skill>();
    public ICollection<RepairRequest> AssignedRepairRequests { get; set; } = new HashSet<RepairRequest>();
    public ICollection<RepairSession> RepairSessions { get; set; } = new HashSet<RepairSession>();
}
