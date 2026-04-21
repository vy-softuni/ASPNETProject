namespace RepairCircle.Tests;

public class BorrowRecordServiceTests
{
    [Fact]
    public async Task GetCreateModelAsync_ReturnsToolSnapshot()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var realtime = new FakeRealtimeNotificationService();
        var service = new BorrowRecordService(db, realtime);

        var model = await service.GetCreateModelAsync(1);

        Assert.NotNull(model);
        Assert.Equal("Cordless Drill", model!.ToolName);
        Assert.True(model.IsAvailable);
    }

    [Fact]
    public async Task CreateAsync_CreatesBorrowRecordAndUpdatesAvailability()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var realtime = new FakeRealtimeNotificationService();
        var service = new BorrowRecordService(db, realtime);

        var id = await service.CreateAsync("user-1", new BorrowRecordCreateInputModel
        {
            ToolId = 1,
            DueDate = DateTime.UtcNow.Date.AddDays(3)
        });

        var tool = await db.Tools.FindAsync(1);
        Assert.True(id > 0);
        Assert.NotNull(tool);
        Assert.Equal(1, tool!.Quantity);
        Assert.Single(realtime.AdminMessages);
        Assert.Single(realtime.ToolAvailabilityChanges);
    }

    [Fact]
    public async Task GetUserRecordsAsync_ReturnsOnlyCurrentUsersRecords()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var realtime = new FakeRealtimeNotificationService();
        var service = new BorrowRecordService(db, realtime);

        var result = await service.GetUserRecordsAsync("user-1");

        Assert.Single(result.Items);
        Assert.Equal("BOR-001", result.Items.First().BorrowReference);
    }

    [Fact]
    public async Task GetByIdForUserAsync_RejectsAnotherUsersRecord()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var realtime = new FakeRealtimeNotificationService();
        var service = new BorrowRecordService(db, realtime);

        var result = await service.GetByIdForUserAsync(1, "user-2");

        Assert.Null(result);
    }
}
