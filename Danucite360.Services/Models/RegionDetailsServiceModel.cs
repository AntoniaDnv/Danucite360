namespace Danucite360.Services.Models;

public class RegionDetailsServiceModel
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public int Population { get; set; }

    public int BudgetYear { get; set; }

    public decimal TotalDemoSpending { get; set; }

    public string Unit { get; set; } = "thousand EUR";

    public string DemoNotice { get; set; } = string.Empty;
}