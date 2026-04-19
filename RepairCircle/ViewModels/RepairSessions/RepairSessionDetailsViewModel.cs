namespace RepairCircle.ViewModels.RepairSessions;

public class RepairSessionDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AvailableSlots { get; set; }
    public int MaxParticipants { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public IReadOnlyCollection<string> VolunteerNames { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> RepairRequestTitles { get; set; } = Array.Empty<string>();
}
