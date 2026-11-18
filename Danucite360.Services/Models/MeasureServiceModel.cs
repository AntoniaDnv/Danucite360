namespace Danucite360.Services.Models;

public class MeasureServiceModel
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string WhatItMeans { get; set; } = string.Empty;
    public string Category { get; set; } = null!;
    public IReadOnlyList<string> Audiences { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> AgeGroups { get; set; } = Array.Empty<string>();
    public decimal? FiscalImpact { get; set; }
    public string Unit { get; set; } = null!;
    public DateTime? EffectiveDate { get; set; }
    public string Status { get; set; } = null!;
    public int BudgetYear { get; set; }
    public bool IsDemo { get; set; }
    public string SourceTitle { get; set; } = null!;
    public string SourceUrl { get; set; } = string.Empty;
}
