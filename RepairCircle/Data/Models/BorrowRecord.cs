using System.ComponentModel.DataAnnotations;
using RepairCircle.Data.Enums;

namespace RepairCircle.Data.Models;

public class BorrowRecord : BaseEntity
{
    public int ToolId { get; set; }
    public Tool Tool { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedDate { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Pending;

    [Required]
    [StringLength(50)]
    public string BorrowReference { get; set; } = null!;
}
