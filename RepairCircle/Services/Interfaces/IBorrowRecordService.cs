using RepairCircle.ViewModels.BorrowRecords;
using RepairCircle.ViewModels.Common;

namespace RepairCircle.Services.Interfaces;

public interface IBorrowRecordService
{
    Task<PagedCollectionViewModel<BorrowRecordListItemViewModel>> GetUserRecordsAsync(string userId, int page = 1, int pageSize = 10);
    Task<BorrowRecordCreateViewModel?> GetCreateModelAsync(int toolId);
    Task<int> CreateAsync(string userId, BorrowRecordCreateInputModel model);
}
