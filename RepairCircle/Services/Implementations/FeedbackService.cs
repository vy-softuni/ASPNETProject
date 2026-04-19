using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Feedbacks;
using RepairCircle.ViewModels.RepairRequests;

namespace RepairCircle.Services.Implementations;

public class FeedbackService : IFeedbackService
{
    private readonly ApplicationDbContext dbContext;

    public FeedbackService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<FeedbackFormViewModel?> GetCreateModelAsync(int repairRequestId, string userId)
    {
        return await dbContext.RepairRequests
            .AsNoTracking()
            .Where(r => r.Id == repairRequestId && r.SubmittedByUserId == userId)
            .Select(r => new FeedbackFormViewModel
            {
                RepairRequestTitle = r.Title,
                Input = new FeedbackFormInputModel
                {
                    RepairRequestId = r.Id,
                    Rating = 5
                }
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int?> CreateAsync(string userId, FeedbackFormInputModel inputModel)
    {
        var requestExists = await dbContext.RepairRequests
            .AnyAsync(r => r.Id == inputModel.RepairRequestId && r.SubmittedByUserId == userId);
        if (!requestExists)
        {
            return null;
        }

        var alreadyLeftFeedback = await dbContext.Feedbacks
            .AnyAsync(f => f.UserId == userId && f.RepairRequestId == inputModel.RepairRequestId);
        if (alreadyLeftFeedback)
        {
            return null;
        }

        var feedback = new Feedback
        {
            UserId = userId,
            RepairRequestId = inputModel.RepairRequestId,
            Rating = inputModel.Rating,
            Comment = inputModel.Comment.Trim()
        };

        await dbContext.Feedbacks.AddAsync(feedback);
        await dbContext.SaveChangesAsync();
        return feedback.Id;
    }

    public async Task<FeedbackFormViewModel?> GetEditModelAsync(int feedbackId, string userId)
    {
        return await dbContext.Feedbacks
            .AsNoTracking()
            .Include(f => f.RepairRequest)
            .Where(f => f.Id == feedbackId && f.UserId == userId)
            .Select(f => new FeedbackFormViewModel
            {
                RepairRequestTitle = f.RepairRequest.Title,
                Input = new FeedbackFormInputModel
                {
                    Id = f.Id,
                    RepairRequestId = f.RepairRequestId,
                    Rating = f.Rating,
                    Comment = f.Comment
                }
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAsync(string userId, FeedbackFormInputModel inputModel)
    {
        var feedback = await dbContext.Feedbacks
            .FirstOrDefaultAsync(f => f.Id == inputModel.Id && f.UserId == userId);
        if (feedback is null)
        {
            return false;
        }

        feedback.Rating = inputModel.Rating;
        feedback.Comment = inputModel.Comment.Trim();
        feedback.ModifiedOn = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string userId, int feedbackId)
    {
        var feedback = await dbContext.Feedbacks
            .FirstOrDefaultAsync(f => f.Id == feedbackId && f.UserId == userId);
        if (feedback is null)
        {
            return false;
        }

        dbContext.Feedbacks.Remove(feedback);
        await dbContext.SaveChangesAsync();
        return true;
    }

public async Task<IReadOnlyCollection<RepairRequestFeedbackViewModel>> GetForRepairRequestAsync(int repairRequestId)
{
    return await dbContext.Feedbacks
        .AsNoTracking()
        .Include(f => f.User)
        .Where(f => f.RepairRequestId == repairRequestId)
        .OrderByDescending(f => f.CreatedOn)
        .Select(f => new RepairRequestFeedbackViewModel
        {
            UserName = f.User.FullName ?? f.User.UserName ?? f.User.Email ?? "Unknown user",
            Rating = f.Rating,
            Comment = f.Comment,
            CreatedOn = f.CreatedOn
        })
        .ToListAsync();
}

}
