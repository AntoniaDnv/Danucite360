namespace Danucite360.Services.Models;

public class CategorySpendingServiceModel
{
    public string CategoryName { get; set; } = null!;

    public string CategorySlug { get; set; } = null!;

    public decimal Amount { get; set; }

    public decimal Percentage { get; set; }

    public string Unit { get; set; } = "thousand EUR";
}