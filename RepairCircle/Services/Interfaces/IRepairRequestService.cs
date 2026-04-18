using RepairCircle.ViewModels.RepairRequests;

namespace RepairCircle.Services.Interfaces;

public interface IRepairRequestService
{
    Task<RepairRequestIndexViewModel> GetAllAsync();
    Task<RepairRequestIndexViewModel> GetMineAsync(string userId);
    Task<RepairRequestDetailsViewModel?> GetByIdAsync(int id);
    Task<RepairRequestCreateViewModel> GetCreateModelAsync();
    Task<int> CreateAsync(RepairRequestCreateInputModel model, string userId);
}
