using System.ComponentModel.DataAnnotations;

namespace Danucite360.Web.ViewModels.Admin;

public class RegionInputModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(120)]
    [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug may contain only lowercase letters, numbers, and dashes.")]
    public string Slug { get; set; } = null!;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 10000000)]
    public int Population { get; set; }

    public bool IsDemo { get; set; } = true;
}