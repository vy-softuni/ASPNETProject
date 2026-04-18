using RepairCircle.ViewModels.BorrowRecords;

namespace RepairCircle.Services.Interfaces;

public interface IBorrowRecordService
{
    Task<IReadOnlyCollection<BorrowRecordListItemViewModel>> GetUserRecordsAsync(string userId);
    Task<BorrowRecordCreateViewModel?> GetCreateModelAsync(int toolId);
    Task<int> CreateAsync(string userId, BorrowRecordCreateInputModel model);
}
