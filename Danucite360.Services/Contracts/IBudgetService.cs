using Danucite360.Services.Models;

namespace Danucite360.Services.Contracts;

public interface IBudgetService
{
    Task<NationalBudgetServiceModel> GetNationalOverviewAsync(int year);

    BudgetExecutionServiceModel GetBudgetExecution(int year);

    Task<IEnumerable<CategorySpendingServiceModel>> GetCategoryOverviewAsync(int year);

    Task<CategoryDetailServiceModel?> GetCategoryDetailAsync(string categorySlug, int year);

    Task<ChartDataServiceModel> GetNationalChartDataAsync(int year);

    Task<ChartDataServiceModel> GetCategoryChartDataAsync(int year);

    Task<ChartDataServiceModel?> GetRegionChartDataAsync(string regionSlug, int year);

    Task<IEnumerable<RegionListServiceModel>> GetMapDataAsync(int year);
}