using Danucite360.Common.Constants;
using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

public class ProjectsController : Controller
{
    private readonly IProjectService projectService;

    public ProjectsController(IProjectService projectService)
    {
        this.projectService = projectService;
    }

    [HttpGet("/projects")]
    public async Task<IActionResult> Index()
    {
        var model = await projectService.GetOverviewAsync(DataConstants.BudgetYear);
        return View(model);
    }

    [HttpGet("/projects/explorer")]
    public async Task<IActionResult> Explorer()
    {
        var model = await projectService.GetAllProjectsAsync(DataConstants.BudgetYear);
        return View(model);
    }

    [HttpGet("/projects/regions/{slug}")]
    public async Task<IActionResult> Region(string slug)
    {
        var model = await projectService.GetRegionProjectsAsync(slug, DataConstants.BudgetYear);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpGet("/projects/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var model = await projectService.GetProjectDetailAsync(slug, DataConstants.BudgetYear);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }
}
