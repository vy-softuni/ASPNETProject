using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Enums;
using RepairCircle.Data.Models;
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

    public async Task<BorrowRecordCreateViewModel?> GetCreateModelAsync(int toolId)
    {
        return await dbContext.Tools
            .AsNoTracking()
            .Include(t => t.Location)
            .Where(t => t.Id == toolId)
            .Select(t => new BorrowRecordCreateViewModel
            {
                ToolName = t.Name,
                LocationName = $"{t.Location.Name} ({t.Location.City})",
                IsAvailable = t.IsAvailable,
                AvailableQuantity = t.Quantity,
                Input = new BorrowRecordCreateInputModel
                {
                    ToolId = t.Id,
                    DueDate = DateTime.UtcNow.Date.AddDays(7)
                }
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(string userId, BorrowRecordCreateInputModel model)
    {
        var tool = await dbContext.Tools.FirstOrDefaultAsync(t => t.Id == model.ToolId);
        if (tool is null || !tool.IsAvailable || tool.Quantity <= 0)
        {
            return 0;
        }

        var record = new BorrowRecord
        {
            ToolId = model.ToolId,
            UserId = userId,
            BorrowDate = DateTime.UtcNow,
            DueDate = model.DueDate.ToUniversalTime(),
            Status = BorrowStatus.Pending,
            BorrowReference = $"BOR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}"
        };

        await dbContext.BorrowRecords.AddAsync(record);
        await dbContext.SaveChangesAsync();
        return record.Id;
    }
}
