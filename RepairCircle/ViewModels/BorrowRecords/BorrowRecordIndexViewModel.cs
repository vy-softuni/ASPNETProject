using RepairCircle.ViewModels.Common;

namespace RepairCircle.ViewModels.BorrowRecords;

public class BorrowRecordIndexViewModel
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }

    public IReadOnlyCollection<string> Statuses { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<BorrowRecordListItemViewModel> Items { get; set; } = Array.Empty<BorrowRecordListItemViewModel>();
    public PaginationViewModel Pagination { get; set; } = new();
}
