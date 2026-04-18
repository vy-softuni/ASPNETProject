using System.ComponentModel.DataAnnotations;

namespace RepairCircle.ViewModels.RepairRequests;

public class RepairRequestCreateInputModel
{
    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string ItemType { get; set; } = string.Empty;

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [Range(1, int.MaxValue)]
    public int LocationId { get; set; }

    public int? RepairSessionId { get; set; }
}
