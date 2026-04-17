using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class Announcement : BaseEntity
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = null!;

    public bool IsImportant { get; set; }
    public bool IsPublished { get; set; } = true;
}
