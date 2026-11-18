using Danucite360.Common.Constants;
using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

public class MeasuresController : Controller
{
    private readonly IMeasureService measureService;

    public MeasuresController(IMeasureService measureService)
    {
        this.measureService = measureService;
    }

    [HttpGet("/measures")]
    public async Task<IActionResult> Index([FromQuery] string[]? role, [FromQuery] string? age)
    {
        // Only accept known tags; anything else from the query string is ignored.
        var roles = (role ?? Array.Empty<string>())
            .Where(r => MeasureAudiences.Selectable.Any(a => a.Key == r))
            .ToArray();
        var ageGroup = MeasureAgeGroups.Selectable.Any(a => a.Key == age) ? age : null;

        ViewData["SelectedRoles"] = roles;
        ViewData["SelectedAge"] = ageGroup;

        var model = await measureService.GetMeasuresAsync(DataConstants.BudgetYear, roles, ageGroup);
        return View(model);
    }

    [HttpGet("/measures/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var model = await measureService.GetMeasureAsync(slug);
        return model == null ? NotFound() : View(model);
    }
}
