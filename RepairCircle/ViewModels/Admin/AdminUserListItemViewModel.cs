namespace RepairCircle.ViewModels.Admin;

public class AdminUserListItemViewModel
{
    public string Id { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string? UserName { get; set; }

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public ICollection<string> Roles { get; set; } = new List<string>();
}
