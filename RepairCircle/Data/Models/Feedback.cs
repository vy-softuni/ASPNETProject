using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class Feedback : BaseEntity
{
    [Required]
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public int RepairRequestId { get; set; }
    public RepairRequest RepairRequest { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; } = null!;
}
