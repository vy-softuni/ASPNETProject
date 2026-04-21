namespace RepairCircle.Tests;

internal sealed class FakeRealtimeNotificationService : IRealtimeNotificationService
{
    public List<string> AdminMessages { get; } = new();
    public List<(int ToolId, int Quantity, bool IsAvailable, string Message)> ToolAvailabilityChanges { get; } = new();

    public Task NotifyAdminNewBorrowRecordAsync(string toolName, string borrowReference, string requestedBy, DateTime requestedOnUtc)
    {
        AdminMessages.Add($"borrow:{toolName}:{requestedBy}:{borrowReference}");
        return Task.CompletedTask;
    }

    public Task NotifyAdminNewRepairRequestAsync(string title, string submittedBy, string locationName, DateTime requestedOnUtc)
    {
        AdminMessages.Add($"repair:{title}:{submittedBy}:{locationName}");
        return Task.CompletedTask;
    }

    public Task NotifyToolAvailabilityChangedAsync(int toolId, int quantity, bool isAvailable, string message)
    {
        ToolAvailabilityChanges.Add((toolId, quantity, isAvailable, message));
        return Task.CompletedTask;
    }
}
