using System.ComponentModel.DataAnnotations;

namespace Danucite360.Data.Models;

/// <summary>
/// A single funding source share for a <see cref="FundedProject"/>
/// (e.g. EU Cohesion Fund, National Budget, Municipal Co-financing).
/// </summary>
public class ProjectFunding
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string SourceName { get; set; } = null!;

    // Amount contributed by this source, in "thousand EUR".
    public decimal Amount { get; set; }

    public int FundedProjectId { get; set; }

    public FundedProject FundedProject { get; set; } = null!;
}
