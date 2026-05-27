using Danucite360.Common.Constants;
using Danucite360.Data.Data;
using Danucite360.Services.Contracts;
using Danucite360.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Services.Implementations;

public class BudgetService : IBudgetService
{
    private readonly ApplicationDbContext context;

    public BudgetService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<NationalBudgetServiceModel> GetNationalOverviewAsync(int year)
    {
        var records = await context.BudgetRecords
            .AsNoTracking()
            .Include(r => r.BudgetSource)
            .Where(r => r.BudgetYear == year && r.RegionId == null)
            .ToListAsync();

        var revenue = records
            .Where(r => r.RecordType == BudgetRecordTypes.NationalRevenue)
            .Sum(r => r.Amount);

        var expenses = records
            .Where(r => r.RecordType == BudgetRecordTypes.NationalExpense)
            .Sum(r => r.Amount);

        var balance = records
            .Where(r => r.RecordType == BudgetRecordTypes.BudgetBalance)
            .Select(r => r.Amount)
            .FirstOrDefault();

        var firstRecord = records.FirstOrDefault();
        var source = firstRecord?.BudgetSource;

        return new NationalBudgetServiceModel
        {
            BudgetYear = year,
            Revenue = revenue,
            Expenses = expenses,
            Balance = balance != 0 ? balance : revenue - expenses,
            Unit = firstRecord?.Unit ?? DataConstants.DefaultUnit,
            SourceTitle = source?.Title ?? "No source available",
            SourceUrl = source?.Url ?? "#"
        };
    }

    public async Task<IEnumerable<CategorySpendingServiceModel>> GetCategoryOverviewAsync(int year)
    {
        var records = await context.BudgetRecords
            .AsNoTracking()
            .Include(r => r.BudgetCategory)
            .Where(r => r.BudgetYear == year &&
                        r.RecordType == BudgetRecordTypes.RegionalExpense)
            .ToListAsync();

        var total = records.Sum(r => r.Amount);

        if (total == 0)
        {
            return Enumerable.Empty<CategorySpendingServiceModel>();
        }

        return records
            .GroupBy(r => new
            {
                r.BudgetCategory.Name,
                r.BudgetCategory.Slug
            })
            .Select(g => new CategorySpendingServiceModel
            {
                CategoryName = g.Key.Name,
                CategorySlug = g.Key.Slug,
                Amount = g.Sum(r => r.Amount),
                Percentage = Math.Round(g.Sum(r => r.Amount) / total * 100, 2),
                Unit = g.First().Unit
            })
            .OrderByDescending(c => c.Amount)
            .ToList();
    }

    public async Task<ChartDataServiceModel> GetNationalChartDataAsync(int year)
    {
        var overview = await GetNationalOverviewAsync(year);

        return new ChartDataServiceModel
        {
            Year = year,
            Labels = new[] { "Revenue", "Expenses", "Budget Balance" },
            Values = new[]
            {
            overview.Revenue,
            overview.Expenses,
            overview.Balance
        },
            Unit = overview.Unit
        };
    }

    public async Task<ChartDataServiceModel> GetCategoryChartDataAsync(int year)
    {
        var categories = (await GetCategoryOverviewAsync(year)).ToList();

        return new ChartDataServiceModel
        {
            Year = year,
            Labels = categories.Select(c => c.CategoryName).ToArray(),
            Values = categories.Select(c => c.Amount).ToArray(),
            Unit = categories.FirstOrDefault()?.Unit ?? DataConstants.DefaultUnit,
            Notice = DataConstants.DemoDatasetNotice
        };
    }

    public async Task<ChartDataServiceModel?> GetRegionChartDataAsync(string regionSlug, int year)
    {
        var region = await context.Regions
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Slug == regionSlug);

        if (region == null)
        {
            return null;
        }

        var records = await context.BudgetRecords
            .AsNoTracking()
            .Include(r => r.BudgetCategory)
            .Where(r => r.BudgetYear == year &&
                        r.RegionId == region.Id &&
                        r.RecordType == BudgetRecordTypes.RegionalExpense)
            .OrderByDescending(r => r.Amount)
            .ToListAsync();

        return new ChartDataServiceModel
        {
            Year = year,
            Labels = records.Select(r => r.BudgetCategory.Name).ToArray(),
            Values = records.Select(r => r.Amount).ToArray(),
            Unit = records.FirstOrDefault()?.Unit ?? DataConstants.DefaultUnit,
            Notice = DataConstants.DemoDatasetNotice
        };
    }

    public async Task<IEnumerable<RegionListServiceModel>> GetMapDataAsync(int year)
    {
        return await context.Regions
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new RegionListServiceModel
            {
                Name = r.Name,
                Slug = r.Slug,
                Population = r.Population,
                TotalDemoSpending = r.BudgetRecords
                    .Where(br => br.BudgetYear == year &&
                                 br.RecordType == BudgetRecordTypes.RegionalExpense)
                    .Sum(br => br.Amount),
                Unit = DataConstants.DefaultUnit
            })
            .ToListAsync();
    }
}