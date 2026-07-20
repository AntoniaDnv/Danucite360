using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

/// <summary>
/// A point-in-time government debt &amp; guarantees snapshot (one per reporting period),
/// e.g. the Ministry of Finance monthly Government Debt bulletin. Amounts in thousand EUR.
/// </summary>
public class DebtSnapshot
{
    public int Id { get; set; }

    [Required]
    [MaxLength(60)]
    public string Period { get; set; } = null!; // e.g. "April 2026"

    public int BudgetYear { get; set; }

    // Data vintage — when the figures are "as of".
    public DateTime AsOf { get; set; }

    [Required]
    [MaxLength(300)]
    public string SourceTitle { get; set; } = null!;

    [MaxLength(30)]
    public string Unit { get; set; } = "thousand EUR";

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

    [MaxLength(20)]
    public string AvgMaturity { get; set; } = string.Empty;

    public ICollection<DebtBreakdownItem> Breakdowns { get; set; } = new List<DebtBreakdownItem>();
    public ICollection<DebtTrendPoint> TrendPoints { get; set; } = new List<DebtTrendPoint>();
    public ICollection<DebtAuction> Auctions { get; set; } = new List<DebtAuction>();
}
