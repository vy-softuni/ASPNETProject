namespace RepairCircle.ViewModels.RepairRequests;

public class RepairRequestListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public string? AssignedVolunteer { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
}
