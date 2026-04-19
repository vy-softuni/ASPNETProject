namespace RepairCircle.ViewModels.Common;

public class MapEmbedViewModel
{
    public string Title { get; set; } = string.Empty;
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public bool HasCoordinates => Latitude.HasValue && Longitude.HasValue;

    public string Query
    {
        get
        {
            if (HasCoordinates)
            {
                return $"{Latitude!.Value},{Longitude!.Value}";
            }

            var parts = new[] { Title, AddressLine, City }
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .Select(part => part!.Trim());

            return string.Join(", ", parts);
        }
    }

    public string EmbedUrl
    {
        get
        {
            var encodedQuery = Uri.EscapeDataString(Query);
            return $"https://maps.google.com/maps?q={encodedQuery}&z=15&output=embed";
        }
    }

    public string OpenInMapsUrl
    {
        get
        {
            var encodedQuery = Uri.EscapeDataString(Query);
            return $"https://www.google.com/maps/search/?api=1&query={encodedQuery}";
        }
    }
}
