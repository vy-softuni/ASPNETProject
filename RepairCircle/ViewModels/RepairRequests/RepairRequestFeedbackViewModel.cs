namespace RepairCircle.ViewModels.RepairRequests;

public class RepairRequestFeedbackViewModel
{
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}
