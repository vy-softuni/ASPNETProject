using RepairCircle.ViewModels.Volunteers;

namespace RepairCircle.Services.Interfaces;

public interface IVolunteerService
{
    Task<IReadOnlyCollection<VolunteerListItemViewModel>> GetApprovedAsync();
    Task<VolunteerBecomeViewModel> GetBecomeViewModelAsync();
    Task<int> BecomeVolunteerAsync(string userId, VolunteerBecomeInputModel inputModel);
}
