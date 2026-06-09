using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

public class FundedProject
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(220)]
    public string Slug { get; set; } = null!;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    // Infrastructure, Education, Healthcare, Energy, Digitalization, Environment
    [Required]
    [MaxLength(50)]
    public string ProjectType { get; set; } = null!;

    // In Planning, Ongoing, Completed
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = null!;

    [Range(0, 100)]
    public int CompletionPercent { get; set; }

    // Total budget in "thousand EUR".
    public decimal Budget { get; set; }

    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "EUR";

    [Required]
    [MaxLength(30)]
    public string Unit { get; set; } = "thousand EUR";

    public int BudgetYear { get; set; }

    public bool IsDemo { get; set; } = true;

    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;

    public int? RegionId { get; set; }

    public Region? Region { get; set; }

    public int BudgetSourceId { get; set; }

    public BudgetSource BudgetSource { get; set; } = null!;

    public ICollection<ProjectFunding> FundingShares { get; set; } = new List<ProjectFunding>();
}
