using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class Announcement : BaseEntity
{
    [Required(ErrorMessage = "Announcement title is required.")]
    [Display(Name = "Title")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 150 characters.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Announcement content is required.")]
    [Display(Name = "Content")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 2000 characters.")]
    public string Content { get; set; } = null!;

    [Display(Name = "Important")]
    public bool IsImportant { get; set; }

    [Display(Name = "Published")]
    public bool IsPublished { get; set; } = true;
}
