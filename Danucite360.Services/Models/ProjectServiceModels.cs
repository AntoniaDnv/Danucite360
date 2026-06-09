namespace Danucite360.Services.Models;

public class ProjectsOverviewServiceModel
{
    public int BudgetYear { get; set; }
    public int TotalProjects { get; set; }
    public decimal TotalValue { get; set; } // thousand EUR
    public int ActiveRegions { get; set; }
    public int InProgress { get; set; }
    public string Unit { get; set; } = "thousand EUR";
}

public class ProjectListItemServiceModel
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string ProjectType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int CompletionPercent { get; set; }
    public decimal Budget { get; set; }
    public string Unit { get; set; } = "thousand EUR";
    public string RegionName { get; set; } = "National";
    public string? RegionSlug { get; set; }
    public string PrimaryFundingSource { get; set; } = string.Empty;
}

public class ProjectFundingShareServiceModel
{
    public string SourceName { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal Percent { get; set; }
}

public class ProjectDetailServiceModel
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string ProjectType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int CompletionPercent { get; set; }
    public decimal Budget { get; set; }
    public string Unit { get; set; } = "thousand EUR";
    public int BudgetYear { get; set; }
    public string RegionName { get; set; } = "National";
    public string? RegionSlug { get; set; }
    public string SourceTitle { get; set; } = string.Empty;
    public IReadOnlyList<ProjectFundingShareServiceModel> FundingShares { get; set; }
        = new List<ProjectFundingShareServiceModel>();
    public IReadOnlyList<ProjectListItemServiceModel> RelatedProjects { get; set; }
        = new List<ProjectListItemServiceModel>();
}

public class CategoryShareServiceModel
{
    public string Name { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal Percent { get; set; }
}

public class RegionProjectsServiceModel
{
    public string RegionName { get; set; } = null!;
    public string RegionSlug { get; set; } = null!;
    public int BudgetYear { get; set; }
    public int ActiveProjects { get; set; }
    public decimal TotalBudget { get; set; }
    public string Unit { get; set; } = "thousand EUR";
    public IReadOnlyList<CategoryShareServiceModel> ByCategory { get; set; }
        = new List<CategoryShareServiceModel>();
    public IReadOnlyList<ProjectListItemServiceModel> Projects { get; set; }
        = new List<ProjectListItemServiceModel>();
}

public class FundingOverviewServiceModel
{
    public int BudgetYear { get; set; }
    public decimal TotalAllocated { get; set; }
    public string Unit { get; set; } = "thousand EUR";
    public int OpenProcedures { get; set; }
    public IReadOnlyList<CategoryShareServiceModel> AllocationBySource { get; set; }
        = new List<CategoryShareServiceModel>();
    public IReadOnlyList<ProjectListItemServiceModel> LargestProjects { get; set; }
        = new List<ProjectListItemServiceModel>();
}
