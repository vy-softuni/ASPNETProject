namespace RepairCircle.Tests;

public class ToolServiceTests
{
    [Fact]
    public async Task GetAllAsync_FiltersByAvailabilityAndSearchTerm()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var service = new ToolService(db);

        var result = await service.GetAllAsync(searchTerm: "drill", onlyAvailable: true);

        Assert.Single(result.Tools);
        Assert.Equal("Cordless Drill", result.Tools.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByCategoryAndLocation()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var service = new ToolService(db);

        var result = await service.GetAllAsync(categoryId: 1, locationId: 1);

        Assert.Single(result.Tools);
        Assert.Equal("Central Hub", result.Tools.First().LocationName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFavoritedStateForCurrentUser()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var service = new ToolService(db);

        var result = await service.GetByIdAsync(1, "user-1");

        Assert.NotNull(result);
        Assert.True(result!.IsFavoritedByCurrentUser);
        Assert.Equal(1, result.FavoritesCount);
    }
}
