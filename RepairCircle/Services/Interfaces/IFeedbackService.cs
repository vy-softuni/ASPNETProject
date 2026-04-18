using RepairCircle.ViewModels.Feedbacks;

namespace RepairCircle.Services.Interfaces;

public interface IFeedbackService
{
    Task<FeedbackFormViewModel?> GetCreateModelAsync(int repairRequestId, string userId);
    Task<int?> CreateAsync(string userId, FeedbackFormInputModel inputModel);
    Task<FeedbackFormViewModel?> GetEditModelAsync(int feedbackId, string userId);
    Task<bool> UpdateAsync(string userId, FeedbackFormInputModel inputModel);
    Task<bool> DeleteAsync(string userId, int feedbackId);
}
