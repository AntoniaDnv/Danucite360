using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

public class BudgetSource
{
    public int Id { get; set; }

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Publisher { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Url { get; set; } = null!;

    public DateTime? PublishedOn { get; set; }

    public DateTime RetrievedOn { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;

    public ICollection<BudgetRecord> BudgetRecords { get; set; } = new List<BudgetRecord>();
}
