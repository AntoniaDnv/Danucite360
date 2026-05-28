using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Danucite360.Web.ViewModels.Admin;

public class BudgetRecordInputModel
{
    [Range(2000, 2100)]
    public int BudgetYear { get; set; } = 2026;

    public decimal Amount { get; set; }

    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "EUR";

    [Required]
    [StringLength(30)]
    public string Unit { get; set; } = "thousand EUR";

    [Required]
    public string RecordType { get; set; } = null!;

    public bool IsDemo { get; set; }

    public int? RegionId { get; set; }

    [Required]
    public int BudgetCategoryId { get; set; }

    [Required]
    public int BudgetSourceId { get; set; }

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> Regions { get; set; } = new List<SelectListItem>();

    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

    public IEnumerable<SelectListItem> Sources { get; set; } = new List<SelectListItem>();

    public IEnumerable<SelectListItem> RecordTypes { get; set; } = new List<SelectListItem>();
}