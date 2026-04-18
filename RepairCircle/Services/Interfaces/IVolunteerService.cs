using RepairCircle.ViewModels.Common;
using RepairCircle.ViewModels.Volunteers;

namespace RepairCircle.Services.Interfaces;

public interface IVolunteerService
{
    Task<PagedCollectionViewModel<VolunteerListItemViewModel>> GetApprovedAsync(int page = 1, int pageSize = 6);
    Task<VolunteerBecomeViewModel> GetBecomeViewModelAsync();
    Task<int> BecomeVolunteerAsync(string userId, VolunteerBecomeInputModel inputModel);
}
