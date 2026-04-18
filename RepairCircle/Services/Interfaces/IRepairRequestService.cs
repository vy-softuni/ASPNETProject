using RepairCircle.ViewModels.RepairRequests;

namespace RepairCircle.Services.Interfaces;

public interface IRepairRequestService
{
    Task<RepairRequestIndexViewModel> GetAllAsync(int page = 1, int pageSize = 6);
    Task<RepairRequestIndexViewModel> GetMineAsync(string userId, int page = 1, int pageSize = 6);
    Task<RepairRequestDetailsViewModel?> GetByIdAsync(int id);
    Task<RepairRequestCreateViewModel> GetCreateModelAsync();
    Task<int> CreateAsync(RepairRequestCreateInputModel model, string userId);
}
