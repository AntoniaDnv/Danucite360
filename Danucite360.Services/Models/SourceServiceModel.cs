namespace Danucite360.Services.Models;

public class SourceServiceModel
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    public string Url { get; set; } = null!;

    public DateTime? PublishedOn { get; set; }

    public DateTime RetrievedOn { get; set; }

    public string Notes { get; set; } = string.Empty;
}