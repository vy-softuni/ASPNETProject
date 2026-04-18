using RepairCircle.ViewModels.BorrowRecords;

namespace RepairCircle.Services.Interfaces;

public interface IBorrowRecordService
{
    Task<IReadOnlyCollection<BorrowRecordListItemViewModel>> GetUserRecordsAsync(string userId);
}
