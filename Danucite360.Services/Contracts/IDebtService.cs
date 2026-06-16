using Danucite360.Services.Models;

namespace Danucite360.Services.Contracts;

public interface IDebtService
{
    DebtOverviewServiceModel GetOverview(int year);
}
