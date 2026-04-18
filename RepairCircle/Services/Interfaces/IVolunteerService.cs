using RepairCircle.ViewModels.Volunteers;

namespace RepairCircle.Services.Interfaces;

public interface IVolunteerService
{
    Task<VolunteerIndexViewModel> GetApprovedAsync(
        string? searchTerm = null,
        int? skillId = null,
        string? experienceLevel = null,
        int page = 1,
        int pageSize = 6);

    Task<VolunteerBecomeViewModel> GetBecomeViewModelAsync();
    Task<int> BecomeVolunteerAsync(string userId, VolunteerBecomeInputModel inputModel);
}
