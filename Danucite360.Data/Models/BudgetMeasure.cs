using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

/// <summary>
/// A budget/policy measure (tax change, benefit, fee) explained in plain language.
/// FiscalImpact is the effect on the budget balance, in thousand EUR per year:
/// negative = costs the budget (less revenue or more spending), positive = brings money in.
/// It is NOT money spent on a project.
/// </summary>
public class BudgetMeasure
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(220)]
    public string Slug { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Summary { get; set; } = null!;

    [MaxLength(2000)]
    public string WhatItMeans { get; set; } = string.Empty;

    [Required]
    [MaxLength(60)]
    public string Category { get; set; } = null!;

    // Semicolon-separated keys from MeasureAudiences, e.g. "parent;employee".
    [Required]
    [MaxLength(200)]
    public string Audiences { get; set; } = null!;

    // Semicolon-separated keys from MeasureAgeGroups, or "all".
    [Required]
    [MaxLength(100)]
    public string AgeGroups { get; set; } = null!;

    public decimal? FiscalImpact { get; set; }

    [Required]
    [MaxLength(30)]
    public string Unit { get; set; } = "thousand EUR";

    public DateTime? EffectiveDate { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = null!;

    public int BudgetYear { get; set; }

    public bool IsDemo { get; set; } = true;

    [Required]
    [MaxLength(300)]
    public string SourceTitle { get; set; } = null!;

    [MaxLength(1000)]
    public string SourceUrl { get; set; } = string.Empty;
}
