using Danucite360.Common.Constants;
using Danucite360.Data.Data;
using Danucite360.Services.Contracts;
using Danucite360.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Services.Implementations;

public class RegionService : IRegionService
{
    private readonly ApplicationDbContext context;

    public RegionService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<RegionListServiceModel>> GetAllAsync()
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
                    .Where(br => br.RecordType == BudgetRecordTypes.RegionalExpense)
                    .Sum(br => br.Amount),
                Unit = DataConstants.DefaultUnit
            })
            .ToListAsync();
    }

    public async Task<RegionDetailsServiceModel?> GetDetailsBySlugAsync(string slug)
    {
        return await context.Regions
            .AsNoTracking()
            .Where(r => r.Slug == slug)
            .Select(r => new RegionDetailsServiceModel
            {
                Name = r.Name,
                Slug = r.Slug,
                Description = r.Description,
                Population = r.Population,
                BudgetYear = DataConstants.BudgetYear,
                TotalDemoSpending = r.BudgetRecords
                    .Where(br => br.RecordType == BudgetRecordTypes.RegionalExpense)
                    .Sum(br => br.Amount),
                Unit = DataConstants.DefaultUnit,
                DemoNotice = DataConstants.DemoDatasetNotice
            })
            .FirstOrDefaultAsync();
    }
}