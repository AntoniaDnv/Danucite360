using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

public class BudgetRecord
{
    public int Id { get; set; }

    [Range(2000, 2100)]
    public int BudgetYear { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "EUR";

    [Required]
    [MaxLength(30)]
    public string Unit { get; set; } = "thousand EUR";

    [Required]
    [MaxLength(50)]
    public string RecordType { get; set; } = null!;
    // NationalRevenue, NationalExpense, RegionalExpense, BudgetBalance

    public bool IsDemo { get; set; }

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    public int? RegionId { get; set; }

    public Region? Region { get; set; }

    public int BudgetCategoryId { get; set; }

    public BudgetCategory BudgetCategory { get; set; } = null!;

    public int BudgetSourceId { get; set; }

    public BudgetSource BudgetSource { get; set; } = null!;
}
