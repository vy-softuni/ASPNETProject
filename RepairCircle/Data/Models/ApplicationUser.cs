using Microsoft.AspNetCore.Identity;

namespace RepairCircle.Data.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public ICollection<RepairRequest> RepairRequests { get; set; } = new HashSet<RepairRequest>();
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new HashSet<BorrowRecord>();
    public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
    public ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();
}
