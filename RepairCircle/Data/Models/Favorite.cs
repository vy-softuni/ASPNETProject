using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class Favorite : BaseEntity
{
    [Required]
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public int ToolId { get; set; }
    public Tool Tool { get; set; } = null!;
}
