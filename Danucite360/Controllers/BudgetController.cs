using Danucite360.Common.Constants;
using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

public class BudgetController : Controller
{
    private readonly IBudgetService budgetService;

    public BudgetController(IBudgetService budgetService)
    {
        this.budgetService = budgetService;
    }

    [HttpGet("/budget/national")]
    public async Task<IActionResult> National()
    {
        var model = await budgetService.GetNationalOverviewAsync(DataConstants.BudgetYear);
        return View(model);
    }

    [HttpGet("/budget/categories")]
    public async Task<IActionResult> Categories()
    {
        var model = await budgetService.GetCategoryOverviewAsync(DataConstants.BudgetYear);
        return View(model);
    }
}