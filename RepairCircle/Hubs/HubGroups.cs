namespace RepairCircle.Hubs;

public static class HubGroups
{
    public const string Administrators = "administrators";

    public static string Tool(int toolId) => $"tool-{toolId}";
}
