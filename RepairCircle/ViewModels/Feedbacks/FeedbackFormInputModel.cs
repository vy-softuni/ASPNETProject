using System.ComponentModel.DataAnnotations;

namespace RepairCircle.ViewModels.Feedbacks;

public class FeedbackFormInputModel
{
    public int Id { get; set; }
    public int RepairRequestId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;
}
