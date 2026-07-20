using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

/// <summary>
/// A named percentage row within a debt snapshot, grouped by <see cref="Section"/>
/// (Instrument, Currency, Interest, Guaranteed).
/// </summary>
public class DebtBreakdownItem
{
    public int Id { get; set; }

    public int DebtSnapshotId { get; set; }
    public DebtSnapshot DebtSnapshot { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string Section { get; set; } = null!;

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = null!;

    public decimal Percent { get; set; }

    public int SortOrder { get; set; }
}

/// <summary>Monthly domestic/external debt for a snapshot's trend chart (thousand EUR).</summary>
public class DebtTrendPoint
{
    public int Id { get; set; }

    public int DebtSnapshotId { get; set; }
    public DebtSnapshot DebtSnapshot { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Month { get; set; } = null!;

    public decimal Domestic { get; set; }
    public decimal External { get; set; }

    public int SortOrder { get; set; }
}

/// <summary>A government securities auction result within a snapshot.</summary>
public class DebtAuction
{
    public int Id { get; set; }

    public int DebtSnapshotId { get; set; }
    public DebtSnapshot DebtSnapshot { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Date { get; set; } = null!;

    [Required]
    [MaxLength(60)]
    public string Type { get; set; } = null!;

    [Required]
    [MaxLength(40)]
    public string Maturity { get; set; } = null!;

    public decimal YieldPercent { get; set; }

    public int SortOrder { get; set; }
}
