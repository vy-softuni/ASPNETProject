namespace RepairCircle.Tests;

public class VolunteerAndSessionServiceTests
{
    [Fact]
    public async Task GetApprovedAsync_ReturnsOnlyApprovedProfilesAndSupportsSkillFilter()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var volunteerService = new VolunteerService(db);

        var result = await volunteerService.GetApprovedAsync(skillId: 2);

        Assert.Single(result.Volunteers);
        Assert.Equal("Volunteer One", result.Volunteers.First().FullName);
    }

    [Fact]
    public async Task BecomeVolunteerAsync_CreatesPendingProfileWithSelectedSkills()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var volunteerService = new VolunteerService(db);

        var id = await volunteerService.BecomeVolunteerAsync("user-2", new VolunteerBecomeInputModel
        {
            Bio = "Can help with soldering",
            ExperienceLevel = ExperienceLevel.Intermediate.ToString(),
            SelectedSkillIds = new List<int> { 1 }
        });

        var profile = await db.VolunteerProfiles.Include(v => v.Skills).FirstOrDefaultAsync(v => v.Id == id);
        Assert.True(id > 0);
        Assert.NotNull(profile);
        Assert.False(profile!.IsApproved);
        Assert.Single(profile.Skills);
    }

    [Fact]
    public async Task GetAllUpcomingAsync_ReturnsOnlyFutureSessions()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var sessionService = new RepairSessionService(db);

        var result = await sessionService.GetAllUpcomingAsync();

        Assert.Single(result.Sessions);
        Assert.Equal("Saturday Repair Cafe", result.Sessions.First().Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsVolunteerAndRequestDetails()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var sessionService = new RepairSessionService(db);

        var result = await sessionService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Contains("Volunteer One", result!.VolunteerNames);
        Assert.Contains("Broken Speaker", result.RepairRequestTitles);
    }
}
