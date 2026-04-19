using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RepairCircle.ViewModels.RepairRequests;

public class RepairRequestCreateInputModel
{
    [Required(ErrorMessage = "Please enter a short title for the repair request.")]
    [Display(Name = "Request title")]
    [StringLength(120, MinimumLength = 5, ErrorMessage = "The title must be between 5 and 120 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please describe the repair issue.")]
    [Display(Name = "Problem description")]
    [StringLength(2000, MinimumLength = 15, ErrorMessage = "The description must be between 15 and 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please specify the item type.")]
    [Display(Name = "Item type")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "The item type must be between 3 and 80 characters.")]
    public string ItemType { get; set; } = string.Empty;

    [Display(Name = "Image path or URL")]
    [StringLength(255, ErrorMessage = "The image path or URL cannot be longer than 255 characters.")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Upload item photo")]
    public IFormFile? UploadedImage { get; set; }

    [Display(Name = "Location")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a location.")]
    public int LocationId { get; set; }

    [Display(Name = "Repair session")]
    public int? RepairSessionId { get; set; }
}
