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

    public async Task<VolunteerIndexViewModel> GetApprovedAsync(
        string? searchTerm = null,
        int? skillId = null,
        string? experienceLevel = null,
        int page = 1,
        int pageSize = 6)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 6 : pageSize;

        var profiles = await dbContext.VolunteerProfiles
            .AsNoTracking()
            .Include(v => v.User)
            .Include(v => v.Skills)
            .Where(v => v.IsApproved)
            .ToListAsync();

        IEnumerable<VolunteerProfile> filteredProfiles = profiles;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            filteredProfiles = filteredProfiles.Where(v =>
                (v.User.FullName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (v.User.UserName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (v.User.Email?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (v.Bio?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                v.Skills.Any(s => s.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        if (skillId.HasValue)
        {
            filteredProfiles = filteredProfiles.Where(v => v.Skills.Any(s => s.Id == skillId.Value));
        }

        if (!string.IsNullOrWhiteSpace(experienceLevel) &&
            Enum.TryParse<ExperienceLevel>(experienceLevel, out var parsedExperienceLevel))
        {
            filteredProfiles = filteredProfiles.Where(v => v.ExperienceLevel == parsedExperienceLevel);
        }

        var totalItems = filteredProfiles.Count();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var volunteers = filteredProfiles
            .OrderBy(v => v.User.FullName ?? v.User.UserName ?? v.User.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VolunteerListItemViewModel
            {
                Id = v.Id,
                FullName = v.User.FullName ?? v.User.UserName ?? v.User.Email ?? "Unknown user",
                Bio = v.Bio,
                ExperienceLevel = v.ExperienceLevel.ToString(),
                Skills = v.Skills.OrderBy(s => s.Name).Select(s => s.Name).ToList()
            })
            .ToList();

        var skills = await dbContext.Skills
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new LookupViewModel
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();

        return new VolunteerIndexViewModel
        {
            SearchTerm = searchTerm,
            SkillId = skillId,
            ExperienceLevel = experienceLevel,
            Skills = skills,
            ExperienceLevels = Enum.GetNames<ExperienceLevel>(),
            Volunteers = volunteers,
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            }
        };
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
