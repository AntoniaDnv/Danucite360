using Danucite360.Common.Constants;
using Danucite360.Data.Data;
using Danucite360.Data.Models;
using Danucite360.Services.Contracts;
using Danucite360.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext context;

    public ProjectService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<ProjectsOverviewServiceModel> GetOverviewAsync(int year)
    {
        var projects = await context.FundedProjects
            .AsNoTracking()
            .Where(p => p.BudgetYear == year)
            .ToListAsync();

        return new ProjectsOverviewServiceModel
        {
            BudgetYear = year,
            TotalProjects = projects.Count,
            TotalValue = projects.Sum(p => p.Budget),
            ActiveRegions = projects.Where(p => p.RegionId != null).Select(p => p.RegionId).Distinct().Count(),
            InProgress = projects.Count(p => p.Status == ProjectStatuses.Ongoing),
            Unit = DataConstants.DefaultUnit
        };
    }

    public async Task<IEnumerable<ProjectListItemServiceModel>> GetAllProjectsAsync(int year)
    {
        var projects = await context.FundedProjects
            .AsNoTracking()
            .Include(p => p.Region)
            .Include(p => p.FundingShares)
            .Where(p => p.BudgetYear == year)
            .OrderByDescending(p => p.Budget)
            .ToListAsync();

        return projects.Select(ToListItem).ToList();
    }

    public async Task<ProjectDetailServiceModel?> GetProjectDetailAsync(string slug, int year)
    {
        var project = await context.FundedProjects
            .AsNoTracking()
            .Include(p => p.Region)
            .Include(p => p.FundingShares)
            .Include(p => p.BudgetSource)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.BudgetYear == year);

        if (project == null)
        {
            return null;
        }

        var shares = project.FundingShares
            .OrderByDescending(f => f.Amount)
            .Select(f => new ProjectFundingShareServiceModel
            {
                SourceName = f.SourceName,
                Amount = f.Amount,
                Percent = project.Budget == 0 ? 0 : Math.Round(f.Amount / project.Budget * 100, 1)
            })
            .ToList();

        // Related: same region, else same type; exclude self.
        var related = await context.FundedProjects
            .AsNoTracking()
            .Include(p => p.Region)
            .Include(p => p.FundingShares)
            .Where(p => p.BudgetYear == year && p.Slug != slug &&
                        (p.RegionId == project.RegionId || p.ProjectType == project.ProjectType))
            .OrderByDescending(p => p.RegionId == project.RegionId)
            .ThenByDescending(p => p.Budget)
            .Take(3)
            .ToListAsync();

        return new ProjectDetailServiceModel
        {
            Title = project.Title,
            Slug = project.Slug,
            Description = project.Description,
            ProjectType = project.ProjectType,
            Status = project.Status,
            CompletionPercent = project.CompletionPercent,
            Budget = project.Budget,
            Unit = project.Unit,
            BudgetYear = project.BudgetYear,
            RegionName = project.Region?.Name ?? "National",
            RegionSlug = project.Region?.Slug,
            SourceTitle = project.BudgetSource?.Title ?? string.Empty,
            FundingShares = shares,
            RelatedProjects = related.Select(ToListItem).ToList()
        };
    }

    public async Task<RegionProjectsServiceModel?> GetRegionProjectsAsync(string regionSlug, int year)
    {
        var region = await context.Regions
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Slug == regionSlug);

        if (region == null)
        {
            return null;
        }

        var projects = await context.FundedProjects
            .AsNoTracking()
            .Include(p => p.Region)
            .Include(p => p.FundingShares)
            .Where(p => p.BudgetYear == year && p.RegionId == region.Id)
            .OrderByDescending(p => p.Budget)
            .ToListAsync();

        var totalBudget = projects.Sum(p => p.Budget);

        var byCategory = projects
            .GroupBy(p => p.ProjectType)
            .Select(g => new CategoryShareServiceModel
            {
                Name = g.Key,
                Amount = g.Sum(p => p.Budget),
                Percent = totalBudget == 0 ? 0 : Math.Round(g.Sum(p => p.Budget) / totalBudget * 100, 1)
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        return new RegionProjectsServiceModel
        {
            RegionName = region.Name,
            RegionSlug = region.Slug,
            BudgetYear = year,
            ActiveProjects = projects.Count(p => p.Status != ProjectStatuses.Completed),
            TotalBudget = totalBudget,
            Unit = DataConstants.DefaultUnit,
            ByCategory = byCategory,
            Projects = projects.Select(ToListItem).ToList()
        };
    }

    public async Task<FundingOverviewServiceModel> GetFundingOverviewAsync(int year)
    {
        var shares = await context.ProjectFundings
            .AsNoTracking()
            .Where(f => f.FundedProject.BudgetYear == year)
            .ToListAsync();

        var total = shares.Sum(f => f.Amount);

        var bySource = shares
            .GroupBy(f => f.SourceName)
            .Select(g => new CategoryShareServiceModel
            {
                Name = g.Key,
                Amount = g.Sum(f => f.Amount),
                Percent = total == 0 ? 0 : Math.Round(g.Sum(f => f.Amount) / total * 100, 1)
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        var largest = await context.FundedProjects
            .AsNoTracking()
            .Include(p => p.Region)
            .Include(p => p.FundingShares)
            .Where(p => p.BudgetYear == year)
            .OrderByDescending(p => p.Budget)
            .Take(5)
            .ToListAsync();

        var openProcedures = await context.FundedProjects
            .CountAsync(p => p.BudgetYear == year && p.Status == ProjectStatuses.InPlanning);

        return new FundingOverviewServiceModel
        {
            BudgetYear = year,
            TotalAllocated = total,
            Unit = DataConstants.DefaultUnit,
            OpenProcedures = openProcedures,
            AllocationBySource = bySource,
            LargestProjects = largest.Select(ToListItem).ToList()
        };
    }

    private static ProjectListItemServiceModel ToListItem(FundedProject p)
    {
        var primary = p.FundingShares
            .OrderByDescending(f => f.Amount)
            .FirstOrDefault();

        return new ProjectListItemServiceModel
        {
            Title = p.Title,
            Slug = p.Slug,
            ProjectType = p.ProjectType,
            Status = p.Status,
            CompletionPercent = p.CompletionPercent,
            Budget = p.Budget,
            Unit = p.Unit,
            RegionName = p.Region?.Name ?? "National",
            RegionSlug = p.Region?.Slug,
            PrimaryFundingSource = primary?.SourceName ?? string.Empty
        };
    }
}
