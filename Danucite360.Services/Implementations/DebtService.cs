using Danucite360.Data.Data;
using Danucite360.Services.Contracts;
using Danucite360.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Services.Implementations;

/// <summary>
/// Reads the latest official government debt snapshot from the database
/// (seeded from the Ministry of Finance Government Debt monthly bulletin).
/// </summary>
public class DebtService : IDebtService
{
    private readonly ApplicationDbContext context;

    public DebtService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<DebtOverviewServiceModel?> GetOverviewAsync(int year)
    {
        var snapshot = await context.DebtSnapshots
            .AsNoTracking()
            .Include(s => s.Breakdowns)
            .Include(s => s.TrendPoints)
            .Include(s => s.Auctions)
            .Where(s => s.BudgetYear == year)
            .OrderByDescending(s => s.AsOf)
            .FirstOrDefaultAsync();

        if (snapshot == null)
        {
            return null;
        }

        List<NamedPercentServiceModel> Section(string section) => snapshot.Breakdowns
            .Where(b => b.Section == section)
            .OrderBy(b => b.SortOrder)
            .Select(b => new NamedPercentServiceModel { Name = b.Name, Percent = b.Percent })
            .ToList();

        var auctions = snapshot.Auctions
            .OrderBy(a => a.SortOrder)
            .Select(a => new AuctionResultServiceModel
            {
                Date = a.Date,
                Type = a.Type,
                Maturity = a.Maturity,
                YieldPercent = a.YieldPercent
            })
            .ToList();

        // Yield curve is derived from the auction results (maturity years -> yield).
        var yieldCurve = snapshot.Auctions
            .Select(a => new YieldPointServiceModel { Years = ParseYears(a.Maturity), YieldPercent = a.YieldPercent })
            .Where(y => y.Years > 0)
            .OrderBy(y => y.Years)
            .ToList();

        return new DebtOverviewServiceModel
        {
            BudgetYear = snapshot.BudgetYear,
            Period = snapshot.Period,
            Unit = snapshot.Unit,
            SourceTitle = snapshot.SourceTitle,

            TotalDebt = snapshot.TotalDebt,
            DebtToGdpPercent = snapshot.DebtToGdpPercent,
            DomesticDebt = snapshot.DomesticDebt,
            DomesticSharePercent = snapshot.DomesticSharePercent,
            ExternalDebt = snapshot.ExternalDebt,
            ExternalSharePercent = snapshot.ExternalSharePercent,
            GuaranteedDebt = snapshot.GuaranteedDebt,
            GuaranteedToGdpPercent = snapshot.GuaranteedToGdpPercent,

            StateDebt = snapshot.StateDebt,
            StateDebtSharePercent = snapshot.StateDebtSharePercent,
            EurDenominatedPercent = snapshot.EurDenominatedPercent,
            FixedRatePercent = snapshot.FixedRatePercent,
            AvgInterestPercent = snapshot.AvgInterestPercent,
            AvgMaturity = snapshot.AvgMaturity,

            Trend = snapshot.TrendPoints
                .OrderBy(t => t.SortOrder)
                .Select(t => new DebtTrendPointServiceModel { Month = t.Month, Domestic = t.Domestic, External = t.External })
                .ToList(),
            InstrumentStructure = Section("Instrument"),
            CurrencyStructure = Section("Currency"),
            InterestStructure = Section("Interest"),
            GuaranteedBreakdown = Section("Guaranteed"),
            Auctions = auctions,
            YieldCurve = yieldCurve
        };
    }

    private static int ParseYears(string maturity)
    {
        var token = maturity.Split(' ').FirstOrDefault();
        return int.TryParse(token, out var years) ? years : 0;
    }
}
