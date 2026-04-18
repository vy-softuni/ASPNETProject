using System.ComponentModel.DataAnnotations;

namespace RepairCircle.ViewModels.BorrowRecords;

public class BorrowRecordCreateInputModel
{
    public int ToolId { get; set; }

    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }
}
