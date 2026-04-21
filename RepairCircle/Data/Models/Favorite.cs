using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RepairCircle.Data.Models;

public class Favorite : BaseEntity
{
    [Required]
    public string UserId { get; set; } = null!;
    [ValidateNever]
    public ApplicationUser User { get; set; } = null!;

    public int ToolId { get; set; }
    [ValidateNever]
    public Tool Tool { get; set; } = null!;
}
