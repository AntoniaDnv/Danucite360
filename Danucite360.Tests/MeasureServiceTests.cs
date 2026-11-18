using Danucite360.Common.Constants;
using Danucite360.Data.Models;
using Danucite360.Services.Implementations;
using Xunit;

namespace Danucite360.Tests;

public class MeasureServiceTests
{
    private static MeasureService CreateService()
    {
        var context = TestDbContextFactory.CreateDbContext();

        BudgetMeasure M(string slug, string audiences, string ages) => new()
        {
            Title = slug, Slug = slug, Summary = slug, Category = "Данъци",
            Audiences = audiences, AgeGroups = ages, Status = MeasureStatuses.Proposed,
            BudgetYear = DataConstants.BudgetYear, SourceTitle = "Test"
        };

        context.BudgetMeasures.AddRange(
            M("child-relief", "parent;employee", "30-50"),
            M("vignette", "driver", "all"),
            M("pension-indexation", "pensioner", "over65"),
            M("euro-rounding", "everyone", "all"));
        context.SaveChanges();

        return new MeasureService(context);
    }

    [Fact]
    public async Task NoFilters_ReturnsAllMeasures()
    {
        var result = await CreateService().GetMeasuresAsync(DataConstants.BudgetYear, null, null);

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public async Task SelectedRoles_MatchAnyRolePlusEveryone()
    {
        var result = await CreateService().GetMeasuresAsync(
            DataConstants.BudgetYear, new[] { MeasureAudiences.Parent, MeasureAudiences.Driver }, null);

        Assert.Equal(new[] { "child-relief", "euro-rounding", "vignette" }, result.Select(m => m.Slug).OrderBy(s => s));
    }

    [Fact]
    public async Task AgeGroup_MatchesThatAgeOrAllAges()
    {
        var result = await CreateService().GetMeasuresAsync(
            DataConstants.BudgetYear, null, MeasureAgeGroups.Over65);

        Assert.Equal(new[] { "euro-rounding", "pension-indexation", "vignette" }, result.Select(m => m.Slug).OrderBy(s => s));
    }

    [Fact]
    public async Task RolesAndAge_AreCombinedWithAnd()
    {
        var result = await CreateService().GetMeasuresAsync(
            DataConstants.BudgetYear, new[] { MeasureAudiences.Parent }, MeasureAgeGroups.Under30);

        Assert.Equal(new[] { "euro-rounding" }, result.Select(m => m.Slug));
    }

    [Fact]
    public async Task GetMeasure_UnknownSlug_ReturnsNull()
    {
        Assert.Null(await CreateService().GetMeasureAsync("does-not-exist"));
    }

    [Fact]
    public async Task GetMeasure_SplitsAudienceTags()
    {
        var m = await CreateService().GetMeasureAsync("child-relief");

        Assert.NotNull(m);
        Assert.Equal(new[] { "parent", "employee" }, m!.Audiences);
    }
}
