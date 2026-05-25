namespace Danucite360.Services.Models;

public class RegionListServiceModel
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int Population { get; set; }

    public decimal TotalDemoSpending { get; set; }

    public string Unit { get; set; } = "thousand EUR";
}