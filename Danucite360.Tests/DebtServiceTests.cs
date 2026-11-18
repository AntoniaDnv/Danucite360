using Danucite360.Common.Constants;
using Danucite360.Services.Implementations;
using Xunit;

namespace Danucite360.Tests;

public class DebtServiceTests
{
    [Fact]
    public async Task GetOverviewAsync_DomesticPlusExternalEqualsTotal()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new DebtService(context);

        var result = await service.GetOverviewAsync(DataConstants.BudgetYear);

        Assert.NotNull(result);
        Assert.Equal(result!.TotalDebt, result.DomesticDebt + result.ExternalDebt);
    }

    [Fact]
    public async Task GetOverviewAsync_TrendReconcilesToTotalsAndEndsAtApril()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new DebtService(context);

        var result = await service.GetOverviewAsync(DataConstants.BudgetYear);

        Assert.NotNull(result);
        var last = result!.Trend[^1];
        Assert.Equal(result.DomesticDebt, last.Domestic);
        Assert.Equal(result.ExternalDebt, last.External);
        Assert.Equal(result.TotalDebt, last.Domestic + last.External);
    }

    [Fact]
    public async Task GetOverviewAsync_StructureBreakdownsSumToHundred()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new DebtService(context);

        var result = await service.GetOverviewAsync(DataConstants.BudgetYear);

        Assert.NotNull(result);
        Assert.Equal(100m, result!.InstrumentStructure.Sum(i => i.Percent));
        Assert.Equal(100m, result.CurrencyStructure.Sum(c => c.Percent));
        Assert.Equal(100m, result.InterestStructure.Sum(i => i.Percent));
        Assert.Equal(100m, result.GuaranteedBreakdown.Sum(g => g.Percent));
    }

    [Fact]
    public async Task GetOverviewAsync_DerivesYieldCurveFromAuctions()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var service = new DebtService(context);

        var result = await service.GetOverviewAsync(DataConstants.BudgetYear);

        Assert.NotNull(result);
        Assert.Equal(2, result!.YieldCurve.Count);
        Assert.Contains(result.YieldCurve, y => y.Years == 10 && y.YieldPercent == 4.18m);
        Assert.True(result.YieldCurve[0].Years < result.YieldCurve[1].Years);
    }
}
