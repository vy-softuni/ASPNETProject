using System.ComponentModel.DataAnnotations;

namespace RepairCircle.ViewModels.Feedbacks;

public class FeedbackFormInputModel
{
    public int Id { get; set; }
    public int RepairRequestId { get; set; }

    [Display(Name = "Rating")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; } = 5;

    [Required(ErrorMessage = "Please add a short comment.")]
    [Display(Name = "Comment")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "The comment must be between 10 and 1000 characters.")]
    public string Comment { get; set; } = string.Empty;
}
