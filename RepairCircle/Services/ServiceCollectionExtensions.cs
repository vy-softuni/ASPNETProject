using Microsoft.Extensions.DependencyInjection;
using RepairCircle.Services.Implementations;
using RepairCircle.Services.Interfaces;

namespace RepairCircle.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepairCircleServices(this IServiceCollection services)
    {
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IToolService, ToolService>();
        services.AddScoped<IRepairRequestService, RepairRequestService>();
        services.AddScoped<IRepairSessionService, RepairSessionService>();
        services.AddScoped<IBorrowRecordService, BorrowRecordService>();
        services.AddScoped<IVolunteerService, VolunteerService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();

        return services;
    }
}
