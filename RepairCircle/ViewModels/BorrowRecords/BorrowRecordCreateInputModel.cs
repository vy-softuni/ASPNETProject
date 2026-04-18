using System.ComponentModel.DataAnnotations;

namespace RepairCircle.ViewModels.BorrowRecords;

public class BorrowRecordCreateInputModel : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "A valid tool must be selected.")]
    public int ToolId { get; set; }

    [Required(ErrorMessage = "Please choose a due date.")]
    [Display(Name = "Preferred due date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DueDate.Date <= DateTime.UtcNow.Date)
        {
            yield return new ValidationResult(
                "The due date must be after today.",
                new[] { nameof(DueDate) });
        }
    }
}
