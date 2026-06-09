using Danucite360.Services.Models;

namespace Danucite360.Services.Contracts;

public interface IProjectService
{
    Task<ProjectsOverviewServiceModel> GetOverviewAsync(int year);

    Task<IEnumerable<ProjectListItemServiceModel>> GetAllProjectsAsync(int year);

    Task<ProjectDetailServiceModel?> GetProjectDetailAsync(string slug, int year);

    Task<RegionProjectsServiceModel?> GetRegionProjectsAsync(string regionSlug, int year);

    Task<FundingOverviewServiceModel> GetFundingOverviewAsync(int year);
}
