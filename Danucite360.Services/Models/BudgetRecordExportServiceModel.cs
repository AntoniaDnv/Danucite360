namespace Danucite360.Services.Models;

public class BudgetRecordExportServiceModel
{
    public int BudgetYear { get; set; }
    public string RecordType { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string? Region { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public string DataStatus { get; set; } = null!; // "official" | "demo"
    public string Source { get; set; } = null!;
    public string SourceUrl { get; set; } = null!;
    public string Notes { get; set; } = string.Empty;
}
