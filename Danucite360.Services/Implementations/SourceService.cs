using Danucite360.Data.Data;
using Danucite360.Services.Contracts;
using Danucite360.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Services.Implementations;

public class SourceService : ISourceService
{
    private readonly ApplicationDbContext context;

    public SourceService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<SourceServiceModel>> GetAllAsync()
    {
        return await context.BudgetSources
            .AsNoTracking()
            .OrderBy(s => s.Title)
            .Select(s => new SourceServiceModel
            {
                Id = s.Id,
                Title = s.Title,
                Publisher = s.Publisher,
                Url = s.Url,
                PublishedOn = s.PublishedOn,
                RetrievedOn = s.RetrievedOn,
                Notes = s.Notes
            })
            .ToListAsync();
    }

    public async Task<SourceServiceModel?> GetByIdAsync(int id)
    {
        return await context.BudgetSources
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SourceServiceModel
            {
                Id = s.Id,
                Title = s.Title,
                Publisher = s.Publisher,
                Url = s.Url,
                PublishedOn = s.PublishedOn,
                RetrievedOn = s.RetrievedOn,
                Notes = s.Notes
            })
            .FirstOrDefaultAsync();
    }
}