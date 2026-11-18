using Danucite360.Common.Constants;
using Danucite360.Data.Data;
using Danucite360.Data.Models;
using Danucite360.Services.Implementations;
using Xunit;

namespace Danucite360.Tests;

public class ProjectEvidenceTests
{
    private static FundedProject Project(string status = ProjectStatuses.Ongoing, decimal budget = 95_000m, decimal? spent = 31_000m, int completion = 40) => new()
    {
        Title = "Тестов проект", Slug = "test-project", ProjectType = ProjectTypes.Infrastructure, Status = status,
        CompletionPercent = completion, Budget = budget, Spent = spent, BudgetYear = DataConstants.BudgetYear, BudgetSourceId = 1
    };

    [Fact]
    public void BuildEvidence_PaymentsSumToSpent()
    {
        var p = Project();

        var evidence = SeedData.BuildEvidence(p);

        Assert.Equal(p.Spent, evidence.Where(e => e.Stage == EvidenceStages.Payment).Sum(e => e.Amount));
    }

    [Fact]
    public void BuildEvidence_StartsWithAllocationOfFullBudget_AndIsChronological()
    {
        var p = Project();

        var evidence = SeedData.BuildEvidence(p);

        Assert.Equal(EvidenceStages.Allocation, evidence[0].Stage);
        Assert.Equal(p.Budget, evidence[0].Amount);
        Assert.Equal(evidence.OrderBy(e => e.OccurredOn).Select(e => e.Stage), evidence.Select(e => e.Stage));
        Assert.All(evidence, e => Assert.True(e.IsDemo));
    }

    [Fact]
    public void BuildEvidence_ProjectInPlanning_HasNoContract()
    {
        var evidence = SeedData.BuildEvidence(Project(ProjectStatuses.InPlanning, 30_000m, 2_100m, 10));

        Assert.DoesNotContain(evidence, e => e.Stage == EvidenceStages.Contract);
    }

    [Fact]
    public async Task GetProjectDetail_ReturnsEvidenceInOrder()
    {
        using var context = TestDbContextFactory.CreateDbContext();
        var p = Project();
        p.Evidence = SeedData.BuildEvidence(p);
        context.FundedProjects.Add(p);
        context.SaveChanges();

        var detail = await new ProjectService(context).GetProjectDetailAsync("test-project", DataConstants.BudgetYear);

        Assert.NotNull(detail);
        Assert.Equal(p.Evidence.Count, detail!.Evidence.Count);
        Assert.Equal(detail.Evidence.OrderBy(e => e.OccurredOn).Select(e => e.Title), detail.Evidence.Select(e => e.Title));
    }
}
