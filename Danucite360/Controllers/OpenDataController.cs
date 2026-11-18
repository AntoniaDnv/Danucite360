using System.Text;
using Danucite360.Common;
using Danucite360.Common.Constants;
using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

/// <summary>Machine-readable data: CSV downloads and JSON endpoints read from the same DB as the dashboards.</summary>
public class OpenDataController : Controller
{
    private readonly IBudgetService budgetService;
    private readonly IProjectService projectService;
    private readonly IMeasureService measureService;

    public OpenDataController(IBudgetService budgetService, IProjectService projectService, IMeasureService measureService)
    {
        this.budgetService = budgetService;
        this.projectService = projectService;
        this.measureService = measureService;
    }

    [HttpGet("/open-data")]
    public IActionResult Index() => View();

    [HttpGet("/open-data/budget-records.csv")]
    public async Task<IActionResult> BudgetRecordsCsv()
    {
        var rows = await budgetService.GetRecordsForExportAsync(DataConstants.BudgetYear);
        return Csv("danucite360-budget-records-2026.csv",
            new[] { "budget_year", "record_type", "category", "region", "amount", "currency", "unit", "data_status", "source", "source_url", "notes" },
            rows.Select(r => new object?[] { r.BudgetYear, r.RecordType, r.Category, r.Region, r.Amount, r.Currency, r.Unit, r.DataStatus, r.Source, r.SourceUrl, r.Notes }));
    }

    [HttpGet("/open-data/projects.csv")]
    public async Task<IActionResult> ProjectsCsv()
    {
        var rows = await projectService.GetAllProjectsAsync(DataConstants.BudgetYear);
        return Csv("danucite360-projects-2026.csv",
            new[] { "slug", "title", "sector", "status", "region", "city", "latitude", "longitude", "budget", "spent", "physical_progress_percent", "unit", "main_funding_source", "data_status" },
            rows.Select(p => new object?[] { p.Slug, p.Title, Labels.ToBg(p.ProjectType), Labels.ToBg(p.Status), p.RegionName, p.City, p.Latitude, p.Longitude, p.Budget, p.Spent, p.CompletionPercent, p.Unit, Labels.ToBg(p.PrimaryFundingSource), p.IsDemo ? "demo" : "official" }));
    }

    [HttpGet("/open-data/measures.csv")]
    public async Task<IActionResult> MeasuresCsv()
    {
        var rows = await measureService.GetMeasuresAsync(DataConstants.BudgetYear, null, null);
        return Csv("danucite360-measures-2026.csv",
            new[] { "slug", "title", "category", "status", "audiences", "age_groups", "fiscal_impact", "unit", "effective_date", "data_status", "source" },
            rows.Select(m => new object?[] { m.Slug, m.Title, m.Category, m.Status, string.Join(";", m.Audiences), string.Join(";", m.AgeGroups), m.FiscalImpact, m.Unit, m.EffectiveDate?.ToString("yyyy-MM-dd"), m.IsDemo ? "demo" : "official", m.SourceTitle }));
    }

    [HttpGet("/api/budget-records")]
    public async Task<IActionResult> BudgetRecordsJson() =>
        Ok(await budgetService.GetRecordsForExportAsync(DataConstants.BudgetYear));

    [HttpGet("/api/projects")]
    public async Task<IActionResult> ProjectsJson() =>
        Ok(await projectService.GetAllProjectsAsync(DataConstants.BudgetYear));

    [HttpGet("/api/measures")]
    public async Task<IActionResult> MeasuresJson([FromQuery] string[]? role, [FromQuery] string? age) =>
        Ok(await measureService.GetMeasuresAsync(DataConstants.BudgetYear, role, age));

    // UTF-8 with BOM so Excel opens Cyrillic text correctly.
    private FileContentResult Csv(string fileName, string[] headers, IEnumerable<object?[]> rows) =>
        File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(CsvWriter.Write(headers, rows))).ToArray(),
            "text/csv; charset=utf-8", fileName);
}
