using Danucite360.Common.Constants;
using Danucite360.Services.Implementations;
using Xunit;

namespace Danucite360.Tests;

public class DebtServiceTests
{
    [Fact]
    public void GetOverview_DomesticPlusExternalEqualsTotal()
    {
        var service = new DebtService();

        var result = service.GetOverview(DataConstants.BudgetYear);

        Assert.Equal(result.TotalDebt, result.DomesticDebt + result.ExternalDebt);
    }

    [Fact]
    public void GetOverview_TrendReconcilesToTotalsAndEndsAtApril()
    {
        var service = new DebtService();

        var result = service.GetOverview(DataConstants.BudgetYear);

        var last = result.Trend[^1];
        Assert.Equal(result.DomesticDebt, last.Domestic);
        Assert.Equal(result.ExternalDebt, last.External);
        Assert.Equal(result.TotalDebt, last.Domestic + last.External);
    }

    [Fact]
    public void GetOverview_StructureBreakdownsSumToHundred()
    {
        var service = new DebtService();

        var result = service.GetOverview(DataConstants.BudgetYear);

        Assert.Equal(100m, result.InstrumentStructure.Sum(i => i.Percent));
        Assert.Equal(100m, result.CurrencyStructure.Sum(c => c.Percent));
        Assert.Equal(100m, result.InterestStructure.Sum(i => i.Percent));
        Assert.Equal(100m, result.GuaranteedBreakdown.Sum(g => g.Percent));
    }
}
