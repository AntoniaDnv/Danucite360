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
    public IActionResult Index()
    {
        var model = debtService.GetOverview(DataConstants.BudgetYear);
        return View(model);
    }
}
