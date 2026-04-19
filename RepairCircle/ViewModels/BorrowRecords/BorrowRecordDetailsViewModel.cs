namespace RepairCircle.ViewModels.BorrowRecords;

public class BorrowRecordDetailsViewModel
{
    public int Id { get; set; }
    public string ToolName { get; set; } = string.Empty;
    public string BorrowReference { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? ToolImageUrl { get; set; }
    public string? ToolDescription { get; set; }
}
