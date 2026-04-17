namespace RepairCircle.Data.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedOn { get; set; }
}
