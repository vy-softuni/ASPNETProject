using System.ComponentModel.DataAnnotations;

namespace RepairCircle.Data.Models;

public class Location : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = null!;

    [Required]
    [StringLength(80)]
    public string City { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    // For Google Maps bonus later
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public ICollection<Tool> Tools { get; set; } = new HashSet<Tool>();
    public ICollection<RepairRequest> RepairRequests { get; set; } = new HashSet<RepairRequest>();
    public ICollection<RepairSession> RepairSessions { get; set; } = new HashSet<RepairSession>();
}
