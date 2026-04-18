namespace RepairCircle.ViewModels.BorrowRecords;

public class BorrowRecordCreateViewModel
{
    public BorrowRecordCreateInputModel Input { get; set; } = new();
    public string ToolName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int AvailableQuantity { get; set; }
}
