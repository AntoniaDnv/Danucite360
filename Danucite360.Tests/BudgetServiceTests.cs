using Danucite360.Common.Constants;
using Danucite360.Services.Implementations;
using Xunit;

namespace Danucite360.Tests;

public class BudgetServiceTests
{
    [Fact]
    public async Task GetNationalOverviewAsync_ReturnsCorrectRevenue()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new BudgetService(context);

        var result = await service.GetNationalOverviewAsync(DataConstants.BudgetYear);

        Assert.Equal(30369776.7m, result.Revenue);
    }

    [Fact]
    public async Task GetNationalOverviewAsync_ReturnsCorrectExpenses()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new BudgetService(context);

        var result = await service.GetNationalOverviewAsync(DataConstants.BudgetYear);

        Assert.Equal(17256463.0m, result.Expenses);
    }

    [Fact]
    public async Task GetNationalOverviewAsync_ReturnsCorrectBudgetBalance()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new BudgetService(context);

        var result = await service.GetNationalOverviewAsync(DataConstants.BudgetYear);

        Assert.Equal(-4578509.4m, result.Balance);
    }

    [Fact]
    public async Task GetRegionChartDataAsync_ReturnsDataForExistingRegion()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new BudgetService(context);

        var result = await service.GetRegionChartDataAsync("sofia", DataConstants.BudgetYear);

        Assert.NotNull(result);
        Assert.Contains("Education", result!.Labels);
        Assert.Contains("Healthcare", result.Labels);
        Assert.Contains(120000m, result.Values);
        Assert.Contains(95000m, result.Values);
    }

    [Fact]
    public async Task GetRegionChartDataAsync_ReturnsNullForInvalidRegion()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new BudgetService(context);

        var result = await service.GetRegionChartDataAsync("invalid-region", DataConstants.BudgetYear);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCategoryOverviewAsync_GroupsRegionalExpensesByCategory()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new BudgetService(context);

        var result = (await service.GetCategoryOverviewAsync(DataConstants.BudgetYear)).ToList();

        var education = result.FirstOrDefault(c => c.CategorySlug == "education");

        Assert.NotNull(education);
        Assert.Equal(190000m, education!.Amount);
    }
}