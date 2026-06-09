namespace Danucite360.Services.Models;

public class RegionCategoryAmountServiceModel
{
    public string RegionName { get; set; } = null!;

    public string RegionSlug { get; set; } = null!;

    public int Population { get; set; }

    public decimal Amount { get; set; }

    // EUR per resident for this category in this region.
    public decimal PerCapita { get; set; }

    // Percentage difference of this region's per-capita vs the category average.
    public decimal VsAveragePercent { get; set; }
}
