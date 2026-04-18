namespace RepairCircle.ViewModels.BorrowRecords;

public class BorrowRecordListItemViewModel
{
    public int Id { get; set; }
    public string ToolName { get; set; } = string.Empty;
    public string BorrowReference { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
}
