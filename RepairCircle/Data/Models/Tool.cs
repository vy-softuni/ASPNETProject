using System.ComponentModel.DataAnnotations;
using RepairCircle.Data.Enums;

namespace RepairCircle.Data.Models;

public class Tool : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(1500)]
    public string Description { get; set; } = null!;

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    public ToolCondition Condition { get; set; } = ToolCondition.Good;

    public bool IsAvailable { get; set; } = true;

    [Range(0, 1000)]
    public int Quantity { get; set; }

    public int ToolCategoryId { get; set; }
    public ToolCategory ToolCategory { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new HashSet<BorrowRecord>();
    public ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();
}
