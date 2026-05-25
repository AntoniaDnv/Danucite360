namespace Danucite360.Services.Models;

public class ChartDataServiceModel
{
    public int Year { get; set; }

    public string[] Labels { get; set; } = Array.Empty<string>();

    public decimal[] Values { get; set; } = Array.Empty<decimal>();

    public string Unit { get; set; } = "thousand EUR";

    public string? Notice { get; set; }
}