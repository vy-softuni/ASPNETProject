namespace RepairCircle.ViewModels.Home;

public class HomeAnnouncementViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsImportant { get; set; }
    public DateTime CreatedOn { get; set; }
}
