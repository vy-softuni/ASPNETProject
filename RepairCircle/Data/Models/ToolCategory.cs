using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class ToolCategory : BaseEntity
{
    [Required]
    [StringLength(60)]
    public string Name { get; set; } = null!;

    [StringLength(250)]
    public string? Description { get; set; }

    public ICollection<Tool> Tools { get; set; } = new HashSet<Tool>();
}
