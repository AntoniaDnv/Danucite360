using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

public class BudgetCategory
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(120)]
    public string Slug { get; set; } = null!;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public ICollection<BudgetRecord> BudgetRecords { get; set; } = new List<BudgetRecord>();
}
