using Danucite360.Services.Contracts;
using Danucite360.Services.Models;

namespace Danucite360.Services.Implementations;

/// <summary>
/// Official government debt data from the Ministry of Finance monthly
/// Government Debt bulletin (April 2026). Figures reported in EUR million;
/// stored here in thousand EUR (× 1000) to reuse the standard money formatter.
/// </summary>
public class DebtService : IDebtService
{
    public DebtOverviewServiceModel GetOverview(int year)
    {
        return new DebtOverviewServiceModel
        {
            BudgetYear = year,
            Period = "April 2026",
            SourceTitle = "Ministry of Finance — Government Debt monthly bulletin (April 2026)",

            TotalDebt = 33_752_300m,
            DebtToGdpPercent = 26.9m,
            DomesticDebt = 8_477_800m,
            DomesticSharePercent = 25.1m,
            ExternalDebt = 25_274_500m,
            ExternalSharePercent = 74.9m,
            GuaranteedDebt = 637_400m,
            GuaranteedToGdpPercent = 0.5m,

            StateDebt = 31_750_000m,
            StateDebtSharePercent = 94.1m,
            EurDenominatedPercent = 96.2m,
            FixedRatePercent = 99.8m,
            AvgInterestPercent = 3.2m,
            AvgMaturity = "8y 5m",

            // Monthly central government debt (Dec 2025 – Apr 2026), thousand EUR.
            // Reconciles at April: 8,477.8 + 25,274.5 = 33,752.3 (€M).
            Trend = new List<DebtTrendPointServiceModel>
            {
                new() { Month = "Dec 25", Domestic = 8_747_400m, External = 24_061_700m },
                new() { Month = "Jan 26", Domestic = 8_440_400m, External = 24_711_800m },
                new() { Month = "Feb 26", Domestic = 7_877_100m, External = 25_599_900m },
                new() { Month = "Mar 26", Domestic = 8_177_400m, External = 25_538_400m },
                new() { Month = "Apr 26", Domestic = 8_477_800m, External = 25_274_500m }
            },

            InstrumentStructure = new List<NamedPercentServiceModel>
            {
                new() { Name = "Securities (International)", Percent = 67.1m },
                new() { Name = "Securities (Domestic)", Percent = 18.6m },
                new() { Name = "Loans", Percent = 8.8m },
                new() { Name = "Deposits", Percent = 5.5m }
            },

            CurrencyStructure = new List<NamedPercentServiceModel>
            {
                new() { Name = "EUR (Euro)", Percent = 96.2m },
                new() { Name = "USD (Dollar)", Percent = 3.8m }
            },

            InterestStructure = new List<NamedPercentServiceModel>
            {
                new() { Name = "Fixed Rate (Predictable)", Percent = 99.8m },
                new() { Name = "Floating Rate", Percent = 0.2m }
            },

            GuaranteedBreakdown = new List<NamedPercentServiceModel>
            {
                new() { Name = "Energy Sector", Percent = 51.8m },
                new() { Name = "EU Programs", Percent = 24.5m },
                new() { Name = "Financial", Percent = 19.6m },
                new() { Name = "Other", Percent = 4.1m }
            },

            Auctions = new List<AuctionResultServiceModel>
            {
                new() { Date = "06.04.2026", Type = "GS (Bonds)", Maturity = "10 Year", YieldPercent = 4.18m },
                new() { Date = "20.04.2026", Type = "GS (Bonds)", Maturity = "5 Year", YieldPercent = 3.33m }
            },

            YieldCurve = new List<YieldPointServiceModel>
            {
                new() { Years = 5, YieldPercent = 3.33m },
                new() { Years = 10, YieldPercent = 4.18m }
            }
        };
    }
}
