using Danucite360.Services.Models;

namespace Danucite360.Services.Contracts;

public interface IMeasureService
{
    /// <summary>
    /// Measures for the year. A measure matches when it targets ANY of the selected
    /// roles (or everyone) AND, if an age group is chosen, covers that age (or all ages).
    /// No selection = all measures.
    /// </summary>
    Task<IReadOnlyList<MeasureServiceModel>> GetMeasuresAsync(int year, IEnumerable<string>? roles, string? ageGroup);

    Task<MeasureServiceModel?> GetMeasureAsync(string slug);
}
