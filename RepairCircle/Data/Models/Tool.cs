using System.ComponentModel.DataAnnotations;
using RepairCircle.Data.Enums;

namespace RepairCircle.Data.Models;

public class Tool : BaseEntity
{
    [Required(ErrorMessage = "Tool name is required.")]
    [Display(Name = "Tool name")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Tool name must be between 3 and 100 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    [Display(Name = "Description")]
    [StringLength(1500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1500 characters.")]
    public string Description { get; set; } = null!;

    [Display(Name = "Image URL")]
    [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters.")]
    [Url(ErrorMessage = "Please enter a valid image URL.")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Condition")]
    public ToolCondition Condition { get; set; } = ToolCondition.Good;

    [Display(Name = "Available for borrowing")]
    public bool IsAvailable { get; set; } = true;

    [Display(Name = "Quantity")]
    [Range(0, 1000, ErrorMessage = "Quantity must be between 0 and 1000.")]
    public int Quantity { get; set; }

    [Display(Name = "Category")]
    public int ToolCategoryId { get; set; }
    public ToolCategory ToolCategory { get; set; } = null!;

    [Display(Name = "Location")]
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new HashSet<BorrowRecord>();
    public ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();
}
