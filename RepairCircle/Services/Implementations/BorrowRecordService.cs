using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.BorrowRecords;

namespace RepairCircle.Services.Implementations;

public class BorrowRecordService : IBorrowRecordService
{
    private readonly ApplicationDbContext dbContext;

    public BorrowRecordService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<BorrowRecordListItemViewModel>> GetUserRecordsAsync(string userId)
    {
        return await dbContext.BorrowRecords
            .AsNoTracking()
            .Include(br => br.Tool)
                .ThenInclude(t => t.Location)
            .Where(br => br.UserId == userId)
            .OrderByDescending(br => br.BorrowDate)
            .Select(br => new BorrowRecordListItemViewModel
            {
                Id = br.Id,
                ToolName = br.Tool.Name,
                BorrowReference = br.BorrowReference,
                BorrowDate = br.BorrowDate,
                DueDate = br.DueDate,
                ReturnedDate = br.ReturnedDate,
                Status = br.Status.ToString(),
                LocationName = br.Tool.Location.Name
            })
            .ToListAsync();
    }
}
