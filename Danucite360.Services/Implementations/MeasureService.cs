using Danucite360.Common.Constants;
using Danucite360.Data.Data;
using Danucite360.Data.Models;
using Danucite360.Services.Contracts;
using Danucite360.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Services.Implementations;

public class MeasureService : IMeasureService
{
    private readonly ApplicationDbContext context;

    public MeasureService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<MeasureServiceModel>> GetMeasuresAsync(int year, IEnumerable<string>? roles, string? ageGroup)
    {
        // shortcut: tags are filtered in memory, fine for tens of measures; move to a join table if it grows to thousands.
        var measures = await context.BudgetMeasures
            .AsNoTracking()
            .Where(m => m.BudgetYear == year)
            .OrderBy(m => m.Title)
            .ToListAsync();

        var roleSet = roles?.Where(r => !string.IsNullOrWhiteSpace(r)).ToHashSet() ?? new HashSet<string>();

        return measures
            .Select(ToModel)
            .Where(m => roleSet.Count == 0
                || m.Audiences.Contains(MeasureAudiences.Everyone)
                || m.Audiences.Any(roleSet.Contains))
            .Where(m => string.IsNullOrWhiteSpace(ageGroup)
                || m.AgeGroups.Contains(MeasureAgeGroups.All)
                || m.AgeGroups.Contains(ageGroup))
            .ToList();
    }

    public async Task<MeasureServiceModel?> GetMeasureAsync(string slug)
    {
        var measure = await context.BudgetMeasures
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Slug == slug);

        return measure == null ? null : ToModel(measure);
    }

    private static MeasureServiceModel ToModel(BudgetMeasure m) => new()
    {
        Title = m.Title,
        Slug = m.Slug,
        Summary = m.Summary,
        WhatItMeans = m.WhatItMeans,
        Category = m.Category,
        Audiences = Split(m.Audiences),
        AgeGroups = Split(m.AgeGroups),
        FiscalImpact = m.FiscalImpact,
        Unit = m.Unit,
        EffectiveDate = m.EffectiveDate,
        Status = m.Status,
        BudgetYear = m.BudgetYear,
        IsDemo = m.IsDemo,
        SourceTitle = m.SourceTitle,
        SourceUrl = m.SourceUrl
    };

    private static string[] Split(string tags) =>
        tags.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
