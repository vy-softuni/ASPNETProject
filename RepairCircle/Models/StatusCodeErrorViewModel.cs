namespace RepairCircle.Models;

public class StatusCodeErrorViewModel
{
    public int StatusCode { get; set; }

    public string? OriginalPath { get; set; }

    public string? OriginalQueryString { get; set; }

    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);

    public string FullOriginalUrl
    {
        get
        {
            if (string.IsNullOrWhiteSpace(OriginalPath))
            {
                return string.Empty;
            }

            return string.IsNullOrWhiteSpace(OriginalQueryString)
                ? OriginalPath
                : $"{OriginalPath}{OriginalQueryString}";
        }
    }
}
