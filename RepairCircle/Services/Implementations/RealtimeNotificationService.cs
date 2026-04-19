using Microsoft.AspNetCore.SignalR;
using RepairCircle.Hubs;
using RepairCircle.Services.Interfaces;

namespace RepairCircle.Services.Implementations;

public class RealtimeNotificationService : IRealtimeNotificationService
{
    private readonly IHubContext<RepairCircleHub> hubContext;

    public RealtimeNotificationService(IHubContext<RepairCircleHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task NotifyToolAvailabilityChangedAsync(int toolId, int quantity, bool isAvailable, string message)
    {
        return hubContext.Clients.Group(HubGroups.Tool(toolId)).SendAsync("ToolAvailabilityChanged", new
        {
            toolId,
            quantity,
            isAvailable,
            message,
            updatedOnUtc = DateTime.UtcNow.ToString("u")
        });
    }

    public Task NotifyAdminNewBorrowRecordAsync(string toolName, string borrowReference, string requestedBy, DateTime requestedOnUtc)
    {
        return hubContext.Clients.Group(HubGroups.Administrators).SendAsync("AdminNotificationReceived", new
        {
            title = "New borrowing request",
            message = $"{requestedBy} requested '{toolName}'. Ref: {borrowReference}.",
            createdOnUtc = requestedOnUtc.ToString("u")
        });
    }

    public Task NotifyAdminNewRepairRequestAsync(string title, string submittedBy, string locationName, DateTime requestedOnUtc)
    {
        return hubContext.Clients.Group(HubGroups.Administrators).SendAsync("AdminNotificationReceived", new
        {
            title = "New repair request",
            message = $"{submittedBy} submitted '{title}' for {locationName}.",
            createdOnUtc = requestedOnUtc.ToString("u")
        });
    }
}
