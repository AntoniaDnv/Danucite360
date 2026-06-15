namespace Danucite360.Services.Models;

/// <summary>
/// Actual budget execution under the Consolidated Fiscal Programme (CFP),
/// from the Ministry of Finance monthly bulletins. Broader in scope than the
/// State Budget Law plan figures (it also covers municipalities, social funds
/// and EU funds), so it is presented separately.
/// </summary>
public class BudgetExecutionServiceModel
{
    public int BudgetYear { get; set; }

    public string Unit { get; set; } = "thousand EUR";

    public string SourceTitle { get; set; } = string.Empty;

    public IReadOnlyList<MonthlyExecutionPointServiceModel> Months { get; set; }
        = new List<MonthlyExecutionPointServiceModel>();

    // Latest reported month snapshot (cumulative, thousand EUR).
    public string LatestMonth { get; set; } = string.Empty;
    public decimal LatestRevenue { get; set; }
    public decimal LatestExpenditure { get; set; }
    public decimal LatestBalance { get; set; }

    // Deficit as a percentage of projected GDP (negative = deficit), if reported.
    public decimal? LatestBalancePercentGdp { get; set; }
}

public class MonthlyExecutionPointServiceModel
{
    public string Month { get; set; } = null!;

    // Cumulative year-to-date revenue, in thousand EUR.
    public decimal CumulativeRevenue { get; set; }
}
