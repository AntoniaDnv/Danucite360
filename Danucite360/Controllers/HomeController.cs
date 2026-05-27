using Danucite360.Common.Constants;
using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

public class HomeController : Controller
{
    private readonly IBudgetService budgetService;
    private readonly IRegionService regionService;

    public HomeController(
        IBudgetService budgetService,
        IRegionService regionService)
    {
        this.budgetService = budgetService;
        this.regionService = regionService;
    }

    public async Task<IActionResult> Index()
    {
        var nationalBudget = await budgetService.GetNationalOverviewAsync(DataConstants.BudgetYear);
        var regions = await regionService.GetAllAsync();

        ViewBag.RegionCount = regions.Count();
        ViewBag.BudgetYear = DataConstants.BudgetYear;

        return View(nationalBudget);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}