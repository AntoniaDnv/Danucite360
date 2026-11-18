using Danucite360.Common.Constants;
using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

public class DebtController : Controller
{
    private readonly IDebtService debtService;

    public DebtController(IDebtService debtService)
    {
        this.debtService = debtService;
    }

    [HttpGet("/debt")]
    public async Task<IActionResult> Index()
    {
        var model = await debtService.GetOverviewAsync(DataConstants.BudgetYear);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }
}
