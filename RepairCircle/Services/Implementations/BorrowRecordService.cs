using Microsoft.EntityFrameworkCore;
using RepairCircle.Common;
using RepairCircle.Data;
using RepairCircle.Data.Enums;
using RepairCircle.Data.Models;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.BorrowRecords;
using RepairCircle.ViewModels.Common;

namespace RepairCircle.Services.Implementations;

public class BorrowRecordService : IBorrowRecordService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IRealtimeNotificationService realtimeNotificationService;

    public BorrowRecordService(ApplicationDbContext dbContext, IRealtimeNotificationService realtimeNotificationService)
    {
        this.dbContext = dbContext;
        this.realtimeNotificationService = realtimeNotificationService;
    }

    public async Task<BorrowRecordIndexViewModel> GetUserRecordsAsync(
        string userId,
        string? searchTerm = null,
        string? status = null,
        int page = 1,
        int pageSize = 10)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var query = dbContext.BorrowRecords
            .AsNoTracking()
            .Where(br => br.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var likeTerm = $"%{searchTerm.Trim()}%";
            query = query.Where(br =>
                EF.Functions.Like(br.Tool.Name, likeTerm) ||
                EF.Functions.Like(br.BorrowReference, likeTerm) ||
                EF.Functions.Like(br.Tool.Location.Name, likeTerm));
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<BorrowStatus>(status, out var parsedStatus))
        {
            query = query.Where(br => br.Status == parsedStatus);
        }

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var recordRows = await query
            .OrderByDescending(br => br.BorrowDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(br => new
            {
                br.Id,
                ToolName = br.Tool.Name,
                br.BorrowReference,
                br.BorrowDate,
                br.DueDate,
                br.ReturnedDate,
                br.Status,
                LocationName = br.Tool.Location.Name
            })
            .ToListAsync();

        var records = recordRows
            .Select(br => new BorrowRecordListItemViewModel
            {
                Id = br.Id,
                ToolName = br.ToolName,
                BorrowReference = br.BorrowReference,
                BorrowDate = br.BorrowDate,
                DueDate = br.DueDate,
                ReturnedDate = br.ReturnedDate,
                Status = br.Status.ToString(),
                LocationName = br.LocationName
            })
            .ToList();

        return new BorrowRecordIndexViewModel
        {
            SearchTerm = searchTerm,
            Status = status,
            Statuses = Enum.GetNames<BorrowStatus>(),
            Items = records,
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            }
        };
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

    public async Task<BorrowRecordDetailsViewModel?> GetByIdForUserAsync(int id, string userId)
    {
        var row = await dbContext.BorrowRecords
            .AsNoTracking()
            .Where(br => br.Id == id && br.UserId == userId)
            .Select(br => new
            {
                br.Id,
                ToolName = br.Tool.Name,
                br.BorrowReference,
                br.BorrowDate,
                br.DueDate,
                br.ReturnedDate,
                br.Status,
                LocationName = br.Tool.Location.Name,
                Address = br.Tool.Location.Address,
                City = br.Tool.Location.City,
                ToolImageUrl = br.Tool.ImageUrl,
                ToolDescription = br.Tool.Description
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        return new BorrowRecordDetailsViewModel
        {
            Id = row.Id,
            ToolName = row.ToolName,
            BorrowReference = row.BorrowReference,
            BorrowDate = row.BorrowDate,
            DueDate = row.DueDate,
            ReturnedDate = row.ReturnedDate,
            Status = row.Status.ToString(),
            LocationName = row.LocationName,
            Address = row.Address,
            City = row.City,
            ToolImageUrl = row.ToolImageUrl,
            ToolDescription = row.ToolDescription
        };
    }

    public async Task<int> CreateAsync(string userId, BorrowRecordCreateInputModel model)
    {
        var tool = await dbContext.Tools.FirstOrDefaultAsync(t => t.Id == model.ToolId);
        if (tool is null || !tool.IsAvailable || tool.Quantity <= 0)
        {
            return 0;
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        var dueDate = DateTime.SpecifyKind(model.DueDate, DateTimeKind.Utc);
        var now = DateTime.UtcNow;

        var record = new BorrowRecord
        {
            ToolId = model.ToolId,
            UserId = userId,
            BorrowDate = now,
            DueDate = dueDate,
            Status = BorrowStatus.Pending,
            BorrowReference = ReferenceCodeGenerator.CreateBorrowReference(now)
        };

        tool.Quantity -= 1;
        tool.IsAvailable = tool.Quantity > 0;
        tool.ModifiedOn = now;

        await dbContext.BorrowRecords.AddAsync(record);
        await dbContext.SaveChangesAsync();

        await realtimeNotificationService.NotifyToolAvailabilityChangedAsync(
            tool.Id,
            tool.Quantity,
            tool.IsAvailable,
            tool.IsAvailable
                ? $"A borrowing request reserved one unit. {tool.Quantity} remaining."
                : "This tool just became unavailable.");

        var requestedBy = user?.FullName ?? user?.UserName ?? user?.Email ?? "A user";
        await realtimeNotificationService.NotifyAdminNewBorrowRecordAsync(tool.Name, record.BorrowReference, requestedBy, now);

        return record.Id;
    }
}
