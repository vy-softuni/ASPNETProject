using RepairCircle.ViewModels.Tools;

namespace RepairCircle.ViewModels.Home;

public class HomeIndexViewModel
{
    public int AvailableToolsCount { get; set; }
    public int ActiveRepairRequestsCount { get; set; }
    public int UpcomingSessionsCount { get; set; }
    public int ApprovedVolunteersCount { get; set; }

    public IReadOnlyCollection<HomeAnnouncementViewModel> LatestAnnouncements { get; set; } = Array.Empty<HomeAnnouncementViewModel>();
    public IReadOnlyCollection<ToolListItemViewModel> FeaturedTools { get; set; } = Array.Empty<ToolListItemViewModel>();
}
