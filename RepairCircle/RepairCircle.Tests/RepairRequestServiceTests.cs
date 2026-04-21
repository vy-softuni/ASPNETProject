namespace RepairCircle.Tests;

public class RepairRequestServiceTests
{
    [Fact]
    public async Task GetCreateModelAsync_LoadsLocationsAndUpcomingSessions()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var realtime = new FakeRealtimeNotificationService();
        var service = new RepairRequestService(db, realtime);

        var model = await service.GetCreateModelAsync();

        Assert.Equal(2, model.Locations.Count);
        Assert.Single(model.RepairSessions);
    }

    [Fact]
    public async Task CreateAsync_CreatesRequestAndNotifiesAdmins()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var realtime = new FakeRealtimeNotificationService();
        var service = new RepairRequestService(db, realtime);

        var id = await service.CreateAsync(new RepairRequestCreateInputModel
        {
            Title = " Broken Lamp ",
            Description = " Needs rewiring ",
            ItemType = " Lamp ",
            LocationId = 1,
            RepairSessionId = 1
        }, "user-1");

        var request = await db.RepairRequests.FindAsync(id);
        Assert.True(id > 0);
        Assert.NotNull(request);
        Assert.Equal("Broken Lamp", request!.Title);
        Assert.Single(realtime.AdminMessages);
    }

    [Fact]
    public async Task GetMineAsync_ReturnsOnlyUsersRequests()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var realtime = new FakeRealtimeNotificationService();
        var service = new RepairRequestService(db, realtime);

        var result = await service.GetMineAsync("user-1");

        Assert.Single(result.Requests);
        Assert.Equal("Broken Speaker", result.Requests.First().Title);
    }

    [Fact]
    public async Task GetByIdAsync_AllowsOwnerToLeaveFeedbackWhenMissing()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        // remove seeded feedback so owner can leave feedback
        var feedback = await db.Feedbacks.FirstAsync();
        db.Feedbacks.Remove(feedback);
        await db.SaveChangesAsync();
        var realtime = new FakeRealtimeNotificationService();
        var service = new RepairRequestService(db, realtime);

        var result = await service.GetByIdAsync(1, "user-1");

        Assert.NotNull(result);
        Assert.True(result!.CanLeaveFeedback);
    }
}
