using RepairCircle.ViewModels.Home;

namespace RepairCircle.Services.Interfaces;

public interface IHomeService
{
    Task<HomeIndexViewModel> GetHomePageDataAsync();
}
