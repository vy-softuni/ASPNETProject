namespace RepairCircle.ViewModels.RepairRequests;

public class RepairRequestDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string RequestReference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public string SubmittedByUserId { get; set; } = string.Empty;
    public string? AssignedVolunteer { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? RepairSessionTitle { get; set; }
    public DateTime RequestedDate { get; set; }
    public IReadOnlyCollection<RepairRequestFeedbackViewModel> Feedback { get; set; } = Array.Empty<RepairRequestFeedbackViewModel>();
    public bool CanLeaveFeedback { get; set; }
}
