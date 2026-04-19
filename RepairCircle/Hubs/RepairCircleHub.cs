using Microsoft.AspNetCore.SignalR;

namespace RepairCircle.Hubs;

public class RepairCircleHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User?.Identity?.IsAuthenticated == true && Context.User.IsInRole("Administrator"))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, HubGroups.Administrators);
        }

        await base.OnConnectedAsync();
    }

    public Task SubscribeToTool(int toolId)
        => Groups.AddToGroupAsync(Context.ConnectionId, HubGroups.Tool(toolId));

    public Task UnsubscribeFromTool(int toolId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, HubGroups.Tool(toolId));
}
