using Danucite360.Services.Implementations;
using Xunit;

namespace Danucite360.Tests;

public class RegionServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsAllRegions()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new RegionService(context);

        var result = (await service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetDetailsBySlugAsync_ReturnsCorrectRegion()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new RegionService(context);

        var result = await service.GetDetailsBySlugAsync("sofia");

        Assert.NotNull(result);
        Assert.Equal("Sofia", result!.Name);
        Assert.Equal(1286000, result.Population);
    }

    [Fact]
    public async Task GetDetailsBySlugAsync_ReturnsNullForInvalidSlug()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new RegionService(context);

        var result = await service.GetDetailsBySlugAsync("not-real");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetDetailsBySlugAsync_ReturnsTotalDemoSpending()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new RegionService(context);

        var result = await service.GetDetailsBySlugAsync("sofia");

        Assert.NotNull(result);
        Assert.Equal(215000m, result!.TotalDemoSpending);
    }
}