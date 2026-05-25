using Danucite360.Services.Models;

namespace Danucite360.Services.Contracts;

public interface IRegionService
{
    Task<IEnumerable<RegionListServiceModel>> GetAllAsync();

    Task<RegionDetailsServiceModel?> GetDetailsBySlugAsync(string slug);
}