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

    // Amount actually paid out so far, thousand EUR (financial progress; CompletionPercent is physical progress).
    public decimal? Spent { get; set; }

    // Approximate city-level location for the map (never an exact site address).
    [MaxLength(100)]
    public string? City { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public int BudgetYear { get; set; }

    public bool IsDemo { get; set; } = true;

    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;

    public int? RegionId { get; set; }

    public Region? Region { get; set; }

    public int BudgetSourceId { get; set; }

    public BudgetSource BudgetSource { get; set; } = null!;

    public ICollection<ProjectFunding> FundingShares { get; set; } = new List<ProjectFunding>();

    public ICollection<ProjectEvidence> Evidence { get; set; } = new List<ProjectEvidence>();
}
