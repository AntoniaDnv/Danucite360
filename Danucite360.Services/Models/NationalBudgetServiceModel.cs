namespace Danucite360.Services.Models;

public class NationalBudgetServiceModel
{
    public int BudgetYear { get; set; }

    public decimal Revenue { get; set; }

    public decimal Expenses { get; set; }

    public decimal Balance { get; set; }

    public string Unit { get; set; } = "thousand EUR";

    public string SourceTitle { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;
}