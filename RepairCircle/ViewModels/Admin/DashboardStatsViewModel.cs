namespace RepairCircle.ViewModels.Admin;

public class DashboardStatsViewModel
{
    public int ToolsCount { get; set; }
    public int AvailableToolsCount { get; set; }
    public int RepairRequestsCount { get; set; }
    public int PendingBorrowRecordsCount { get; set; }
    public int ApprovedVolunteersCount { get; set; }
    public int UpcomingSessionsCount { get; set; }

    public int RepairedRequestsCount { get; set; }
    public int ActiveBorrowedToolsCount { get; set; }

    public ICollection<DashboardChartPointViewModel> RepairRequestsLastSixMonths { get; set; } = new List<DashboardChartPointViewModel>();
    public ICollection<DashboardChartPointViewModel> BorrowRecordsLastSixMonths { get; set; } = new List<DashboardChartPointViewModel>();
    public ICollection<DashboardChartPointViewModel> TopBorrowedToolCategories { get; set; } = new List<DashboardChartPointViewModel>();
}
