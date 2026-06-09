using Danucite360.Common.Constants;
using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

public class FundingController : Controller
{
    private readonly IProjectService projectService;

    public FundingController(IProjectService projectService)
    {
        this.projectService = projectService;
    }

    [HttpGet("/funding")]
    public async Task<IActionResult> Index()
    {
        var model = await projectService.GetFundingOverviewAsync(DataConstants.BudgetYear);
        return View(model);
    }
}
