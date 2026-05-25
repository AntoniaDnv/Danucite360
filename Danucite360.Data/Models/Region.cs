using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

public class Region
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(120)]
    public string Slug { get; set; } = null!;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 10000000)]
    public int Population { get; set; }

    public bool IsDemo { get; set; } = true;

    public ICollection<BudgetRecord> BudgetRecords { get; set; } = new List<BudgetRecord>();
}
