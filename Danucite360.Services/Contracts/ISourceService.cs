using Danucite360.Services.Models;

namespace Danucite360.Services.Contracts;

public interface ISourceService
{
    Task<IEnumerable<SourceServiceModel>> GetAllAsync();

    Task<SourceServiceModel?> GetByIdAsync(int id);
}