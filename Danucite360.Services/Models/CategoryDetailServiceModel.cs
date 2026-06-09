namespace Danucite360.Services.Models;

public class CategoryDetailServiceModel
{
    public string CategorySlug { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public int BudgetYear { get; set; }

    // Total demo spending for this category across all regions (thousand EUR).
    public decimal TotalAmount { get; set; }

    // Share of total demo regional spending.
    public decimal Percentage { get; set; }

    // EUR per resident across all regions.
    public decimal PerCapita { get; set; }

    public string Unit { get; set; } = "thousand EUR";

    public IReadOnlyList<RegionCategoryAmountServiceModel> Regions { get; set; }
        = new List<RegionCategoryAmountServiceModel>();
}
