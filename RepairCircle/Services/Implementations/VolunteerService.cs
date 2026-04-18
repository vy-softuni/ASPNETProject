using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Enums;
using RepairCircle.Data.Models;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Common;
using RepairCircle.ViewModels.Volunteers;

namespace RepairCircle.Services.Implementations;

public class VolunteerService : IVolunteerService
{
    private readonly ApplicationDbContext dbContext;

    public VolunteerService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<VolunteerListItemViewModel>> GetApprovedAsync()
    {
        return await dbContext.VolunteerProfiles
            .AsNoTracking()
            .Include(v => v.User)
            .Include(v => v.Skills)
            .Where(v => v.IsApproved)
            .OrderBy(v => v.User.FullName)
            .Select(v => new VolunteerListItemViewModel
            {
                Id = v.Id,
                FullName = v.User.FullName ?? v.User.UserName ?? v.User.Email ?? "Unknown user",
                Bio = v.Bio,
                ExperienceLevel = v.ExperienceLevel.ToString(),
                Skills = v.Skills.OrderBy(s => s.Name).Select(s => s.Name).ToList()
            })
            .ToListAsync();
    }

    public async Task<VolunteerBecomeViewModel> GetBecomeViewModelAsync()
    {
        var skills = await dbContext.Skills
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new LookupViewModel
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();

        return new VolunteerBecomeViewModel
        {
            Skills = skills,
            ExperienceLevels = Enum.GetNames<ExperienceLevel>(),
            Input = new VolunteerBecomeInputModel()
        };
    }

    public async Task<int> BecomeVolunteerAsync(string userId, VolunteerBecomeInputModel inputModel)
    {
        if (!Enum.TryParse<ExperienceLevel>(inputModel.ExperienceLevel, out var experienceLevel))
        {
            return 0;
        }

        var selectedSkills = await dbContext.Skills
            .Where(s => inputModel.SelectedSkillIds.Contains(s.Id))
            .ToListAsync();

        if (selectedSkills.Count == 0)
        {
            return 0;
        }

        var existingVolunteerProfile = await dbContext.VolunteerProfiles
            .Include(v => v.Skills)
            .FirstOrDefaultAsync(v => v.UserId == userId);

        if (existingVolunteerProfile != null)
        {
            existingVolunteerProfile.Bio = string.IsNullOrWhiteSpace(inputModel.Bio) ? null : inputModel.Bio.Trim();
            existingVolunteerProfile.ExperienceLevel = experienceLevel;
            existingVolunteerProfile.Skills.Clear();

            foreach (var skill in selectedSkills)
            {
                existingVolunteerProfile.Skills.Add(skill);
            }

            await dbContext.SaveChangesAsync();
            return existingVolunteerProfile.Id;
        }

        var volunteerProfile = new VolunteerProfile
        {
            UserId = userId,
            Bio = string.IsNullOrWhiteSpace(inputModel.Bio) ? null : inputModel.Bio.Trim(),
            ExperienceLevel = experienceLevel,
            IsApproved = false
        };

        foreach (var skill in selectedSkills)
        {
            volunteerProfile.Skills.Add(skill);
        }

        await dbContext.VolunteerProfiles.AddAsync(volunteerProfile);
        await dbContext.SaveChangesAsync();

        return volunteerProfile.Id;
    }
}
