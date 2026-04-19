namespace RepairCircle.Services.Interfaces;

public interface IRealtimeNotificationService
{
    Task NotifyToolAvailabilityChangedAsync(int toolId, int quantity, bool isAvailable, string message);

    Task NotifyAdminNewBorrowRecordAsync(string toolName, string borrowReference, string requestedBy, DateTime requestedOnUtc);

    Task NotifyAdminNewRepairRequestAsync(string title, string submittedBy, string locationName, DateTime requestedOnUtc);
}
