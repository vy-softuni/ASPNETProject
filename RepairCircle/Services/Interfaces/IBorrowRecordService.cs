using RepairCircle.ViewModels.BorrowRecords;

namespace RepairCircle.Services.Interfaces;

public interface IBorrowRecordService
{
    Task<BorrowRecordIndexViewModel> GetUserRecordsAsync(
        string userId,
        string? searchTerm = null,
        string? status = null,
        int page = 1,
        int pageSize = 10);

    Task<BorrowRecordCreateViewModel?> GetCreateModelAsync(int toolId);
    Task<int> CreateAsync(string userId, BorrowRecordCreateInputModel model);
}
