namespace RepairCircle.Tests;

public class FeedbackServiceTests
{
    [Fact]
    public async Task GetCreateModelAsync_ReturnsOnlyOwnersRequest()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var service = new FeedbackService(db);

        var model = await service.GetCreateModelAsync(1, "user-1");
        var missing = await service.GetCreateModelAsync(2, "user-1");

        Assert.NotNull(model);
        Assert.Null(missing);
    }

    [Fact]
    public async Task CreateUpdateDeleteAsync_WorkAsExpected()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var existing = await db.Feedbacks.FirstAsync();
        db.Feedbacks.Remove(existing);
        await db.SaveChangesAsync();
        var service = new FeedbackService(db);

        var id = await service.CreateAsync("user-1", new FeedbackFormInputModel
        {
            RepairRequestId = 1,
            Rating = 4,
            Comment = " Helpful team "
        });
        Assert.NotNull(id);

        var updated = await service.UpdateAsync("user-1", new FeedbackFormInputModel
        {
            Id = id!.Value,
            RepairRequestId = 1,
            Rating = 5,
            Comment = "Great result"
        });
        var feedback = await db.Feedbacks.FindAsync(id);
        Assert.True(updated);
        Assert.Equal(5, feedback!.Rating);

        var deleted = await service.DeleteAsync("user-1", id!.Value);
        Assert.True(deleted);
        Assert.Null(await db.Feedbacks.FindAsync(id.Value));
    }
}
