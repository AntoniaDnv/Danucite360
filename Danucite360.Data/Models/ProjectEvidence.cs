using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

/// <summary>
/// One documented step in a project's life (allocation, procurement, contract, payment, progress report).
/// Amount is in thousand EUR; payments add up to the project's Spent.
/// </summary>
public class ProjectEvidence
{
    public int Id { get; set; }

    public int FundedProjectId { get; set; }

    public FundedProject FundedProject { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Stage { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    public DateTime OccurredOn { get; set; }

    public decimal? Amount { get; set; }

    [Required]
    [MaxLength(300)]
    public string DocumentTitle { get; set; } = null!;

    [MaxLength(1000)]
    public string DocumentUrl { get; set; } = string.Empty;

    public bool IsDemo { get; set; } = true;
}
