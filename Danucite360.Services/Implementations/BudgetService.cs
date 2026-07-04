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
            .Include(r => r.BudgetCategory)
            .Where(r => r.BudgetYear == year && r.RegionId == null)
            .ToListAsync();

        var revenueRecords = records
            .Where(r => r.RecordType == BudgetRecordTypes.NationalRevenue)
            .ToList();

        var revenue = revenueRecords.Sum(r => r.Amount);

        var revenueBreakdown = revenueRecords
            .OrderByDescending(r => r.Amount)
            .Select(r => new CategorySpendingServiceModel
            {
                CategoryName = r.BudgetCategory.Name,
                CategorySlug = r.BudgetCategory.Slug,
                Amount = r.Amount,
                Percentage = revenue == 0 ? 0 : Math.Round(r.Amount / revenue * 100, 2),
                Unit = r.Unit
            })
            .ToList();

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
            SourceUrl = source?.Url ?? "#",
            RevenueBreakdown = revenueBreakdown
        };
    }

    public BudgetExecutionServiceModel GetBudgetExecution(int year)
    {
        // Actual cumulative execution from the Ministry of Finance monthly CFP
        // bulletins (Jan–Apr 2026). Figures reported in EUR million; stored here
        // in thousand EUR (× 1000) to reuse the standard money formatter.
        var months = new List<MonthlyExecutionPointServiceModel>
        {
            new() { Month = "Jan", CumulativeRevenue = 3_606_800m },
            new() { Month = "Feb", CumulativeRevenue = 6_845_000m },
            new() { Month = "Mar", CumulativeRevenue = 10_038_400m },
            new() { Month = "Apr", CumulativeRevenue = 14_071_100m }
        };

        return new BudgetExecutionServiceModel
        {
            BudgetYear = year,
            Unit = DataConstants.DefaultUnit,
            SourceTitle = "Ministry of Finance — monthly bulletins on the Consolidated Fiscal Programme (2026)",
            Months = months,
            LatestMonth = "end-April",
            LatestRevenue = 14_071_100m,
            LatestExpenditure = 15_832_300m,
            LatestBalance = -1_761_200m,
            LatestBalancePercentGdp = -1.4m
        };
    }

    public StateBudgetExecutionServiceModel GetStateBudgetExecution(int year)
    {
        // Actual state budget execution to end-April 2026 (MoF monthly reports),
        // reported in EUR million; stored in thousand EUR (× 1000). All component
        // lists sum exactly to the corresponding total.
        const decimal revenue = 7_917_200m;
        const decimal expenditure = 9_882_300m; // expenses & transfers + EU contribution

        var revenueParts = new (string Name, decimal Amount)[]
        {
            ("VAT", 3_848_100m),
            ("Personal Income Tax", 1_517_800m),
            ("Excise Duties", 1_084_600m),
            ("Corporate Tax", 538_200m),
            ("Other Tax Revenue", 186_500m),
            ("Non-tax Revenue", 708_500m),
            ("Grants & Aid", 33_500m)
        };

        var expenditureParts = new (string Name, decimal Amount)[]
        {
            ("Transfers to Social Insurance Funds", 2_798_600m),
            ("Transfers to Municipalities", 2_239_500m),
            ("Personnel", 2_202_000m),
            ("Maintenance", 640_000m),
            ("EU Budget Contribution", 351_800m),
            ("Social Spending & Scholarships", 341_700m),
            ("Subsidies", 303_200m),
            ("Capital Expenses", 267_700m),
            ("Interest", 229_600m),
            ("Other Transfers & Reserves", 508_200m)
        };

        List<CategorySpendingServiceModel> Build(IEnumerable<(string Name, decimal Amount)> parts, decimal total) =>
            parts.Select(p => new CategorySpendingServiceModel
            {
                CategoryName = p.Name,
                Amount = p.Amount,
                Percentage = total == 0 ? 0 : Math.Round(p.Amount / total * 100, 1),
                Unit = DataConstants.DefaultUnit
            }).ToList();

        return new StateBudgetExecutionServiceModel
        {
            BudgetYear = year,
            Period = "end-April 2026",
            Unit = DataConstants.DefaultUnit,
            SourceTitle = "Ministry of Finance — monthly State Budget execution reports (Apr 2026)",
            Revenue = revenue,
            Expenditure = expenditure,
            Balance = revenue - expenditure,
            RevenueBreakdown = Build(revenueParts, revenue),
            ExpenditureBreakdown = Build(expenditureParts, expenditure)
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

    public async Task<CategoryDetailServiceModel?> GetCategoryDetailAsync(string categorySlug, int year)
    {
        var category = await context.BudgetCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == categorySlug);

        if (category == null)
        {
            return null;
        }

        var records = await context.BudgetRecords
            .AsNoTracking()
            .Include(r => r.Region)
            .Where(r => r.BudgetYear == year &&
                        r.RecordType == BudgetRecordTypes.RegionalExpense &&
                        r.BudgetCategoryId == category.Id)
            .ToListAsync();

        // Only regional spending categories have detail pages.
        if (records.Count == 0)
        {
            return null;
        }

        var totalAmount = records.Sum(r => r.Amount);

        var allRegionalTotal = await context.BudgetRecords
            .AsNoTracking()
            .Where(r => r.BudgetYear == year &&
                        r.RecordType == BudgetRecordTypes.RegionalExpense)
            .SumAsync(r => r.Amount);

        var totalPopulation = records
            .Where(r => r.Region != null)
            .Sum(r => (long)r.Region!.Population);

        var perCapita = totalPopulation > 0 ? totalAmount * 1000m / totalPopulation : 0m;

        var regions = records
            .Where(r => r.Region != null)
            .Select(r => new RegionCategoryAmountServiceModel
            {
                RegionName = r.Region!.Name,
                RegionSlug = r.Region.Slug,
                Population = r.Region.Population,
                Amount = r.Amount,
                PerCapita = r.Region.Population > 0 ? r.Amount * 1000m / r.Region.Population : 0m
            })
            .OrderByDescending(r => r.PerCapita)
            .ToList();

        foreach (var region in regions)
        {
            region.VsAveragePercent = perCapita == 0
                ? 0
                : Math.Round((region.PerCapita - perCapita) / perCapita * 100, 1);
        }

        return new CategoryDetailServiceModel
        {
            CategorySlug = category.Slug,
            CategoryName = category.Name,
            Description = category.Description,
            BudgetYear = year,
            TotalAmount = totalAmount,
            Percentage = allRegionalTotal == 0 ? 0 : Math.Round(totalAmount / allRegionalTotal * 100, 2),
            PerCapita = perCapita,
            Unit = records.First().Unit,
            Regions = regions
        };
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