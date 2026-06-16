namespace Danucite360.Services.Models;

/// <summary>
/// Government debt & guarantees snapshot from the Ministry of Finance monthly
/// Government Debt bulletin (April 2026). Official data, EUR-denominated.
/// Amounts are stored in "thousand EUR" so they reuse the standard formatter.
/// </summary>
public class DebtOverviewServiceModel
{
    public int BudgetYear { get; set; }
    public string Period { get; set; } = "April 2026";
    public string Unit { get; set; } = "thousand EUR";
    public string SourceTitle { get; set; } = string.Empty;

    public decimal TotalDebt { get; set; }
    public decimal DebtToGdpPercent { get; set; }
    public decimal DomesticDebt { get; set; }
    public decimal DomesticSharePercent { get; set; }
    public decimal ExternalDebt { get; set; }
    public decimal ExternalSharePercent { get; set; }
    public decimal GuaranteedDebt { get; set; }
    public decimal GuaranteedToGdpPercent { get; set; }

    public decimal StateDebt { get; set; }
    public decimal StateDebtSharePercent { get; set; }
    public decimal EurDenominatedPercent { get; set; }
    public decimal FixedRatePercent { get; set; }
    public decimal AvgInterestPercent { get; set; }
    public string AvgMaturity { get; set; } = string.Empty;

    public IReadOnlyList<DebtTrendPointServiceModel> Trend { get; set; } = new List<DebtTrendPointServiceModel>();
    public IReadOnlyList<NamedPercentServiceModel> InstrumentStructure { get; set; } = new List<NamedPercentServiceModel>();
    public IReadOnlyList<NamedPercentServiceModel> CurrencyStructure { get; set; } = new List<NamedPercentServiceModel>();
    public IReadOnlyList<NamedPercentServiceModel> InterestStructure { get; set; } = new List<NamedPercentServiceModel>();
    public IReadOnlyList<NamedPercentServiceModel> GuaranteedBreakdown { get; set; } = new List<NamedPercentServiceModel>();
    public IReadOnlyList<AuctionResultServiceModel> Auctions { get; set; } = new List<AuctionResultServiceModel>();
    public IReadOnlyList<YieldPointServiceModel> YieldCurve { get; set; } = new List<YieldPointServiceModel>();
}

public class DebtTrendPointServiceModel
{
    public string Month { get; set; } = null!;
    public decimal Domestic { get; set; } // thousand EUR
    public decimal External { get; set; } // thousand EUR
}

public class NamedPercentServiceModel
{
    public string Name { get; set; } = null!;
    public decimal Percent { get; set; }
}

public class AuctionResultServiceModel
{
    public string Date { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Maturity { get; set; } = null!;
    public decimal YieldPercent { get; set; }
}

public class YieldPointServiceModel
{
    public int Years { get; set; }
    public decimal YieldPercent { get; set; }
}
