using System.ComponentModel.DataAnnotations;
using RepairCircle.Data.Enums;

namespace RepairCircle.Data.Models;

public class BorrowRecord : BaseEntity, IValidatableObject
{
    public int ToolId { get; set; }
    public Tool Tool { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    [Display(Name = "Borrow date")]
    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Due date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Display(Name = "Returned date")]
    [DataType(DataType.Date)]
    public DateTime? ReturnedDate { get; set; }

    [Display(Name = "Status")]
    public BorrowStatus Status { get; set; } = BorrowStatus.Pending;

    [Required]
    [Display(Name = "Borrow reference")]
    [StringLength(50)]
    public string BorrowReference { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DueDate.Date <= BorrowDate.Date)
        {
            yield return new ValidationResult(
                "Due date must be after the borrow date.",
                new[] { nameof(DueDate) });
        }

        if (ReturnedDate.HasValue && ReturnedDate.Value.Date < BorrowDate.Date)
        {
            yield return new ValidationResult(
                "Returned date cannot be before the borrow date.",
                new[] { nameof(ReturnedDate) });
        }
    }
}
