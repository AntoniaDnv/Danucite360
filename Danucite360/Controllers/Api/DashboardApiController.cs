using Danucite360.Common.Constants;
using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers.Api;

[ApiController]
[Route("api/dashboard")]
public class DashboardApiController : ControllerBase
{
    private readonly IBudgetService budgetService;

    public DashboardApiController(IBudgetService budgetService)
    {
        this.budgetService = budgetService;
    }

    [HttpGet("national")]
    public async Task<IActionResult> National()
    {
        var data = await budgetService.GetNationalChartDataAsync(DataConstants.BudgetYear);
        return Ok(data);
    }


    [HttpGet("categories")]
    public async Task<IActionResult> Categories()
    {
        var data = await budgetService.GetCategoryChartDataAsync(DataConstants.BudgetYear);
        return Ok(data);
    }

    [HttpGet("regions")]
    public async Task<IActionResult> Regions()
    {
        var data = await budgetService.GetMapDataAsync(DataConstants.BudgetYear);
        return Ok(data);
    }

    [HttpGet("regions/{slug}")]
    public async Task<IActionResult> Region(string slug)
    {
        var data = await budgetService.GetRegionChartDataAsync(slug, DataConstants.BudgetYear);

        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }
}