namespace RepairCircle.Tests;

public class FavoriteServiceTests
{
    [Fact]
    public async Task GetUserFavoritesAsync_ReturnsSeededFavorite()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var service = new FavoriteService(db);

        var result = await service.GetUserFavoritesAsync("user-1");

        Assert.Single(result.Items);
        Assert.Equal("Cordless Drill", result.Items.First().ToolName);
    }

    [Fact]
    public async Task AddAsync_AddsFavoriteOnlyWhenMissing()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var service = new FavoriteService(db);

        await service.AddAsync("user-1", 2);
        await service.AddAsync("user-1", 2);

        Assert.Equal(2, await db.Favorites.CountAsync(f => f.UserId == "user-1"));
    }

    [Fact]
    public async Task RemoveAsync_RemovesMatchingFavorite()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var service = new FavoriteService(db);

        await service.RemoveAsync("user-1", 1);

        Assert.Empty(await db.Favorites.Where(f => f.UserId == "user-1" && f.ToolId == 1).ToListAsync());
    }

    [Fact]
    public async Task ToggleAsync_AddsThenRemovesFavorite()
    {
        await using var db = TestDbFactory.CreateContext();
        await TestDbFactory.SeedBasicAsync(db);
        var service = new FavoriteService(db);

        var removed = await service.ToggleAsync("user-1", 1);
        var added = await service.ToggleAsync("user-1", 1);

        Assert.NotNull(removed);
        Assert.False(removed!.IsFavorited);
        Assert.NotNull(added);
        Assert.True(added!.IsFavorited);
    }
}
