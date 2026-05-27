using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

public class RegionsController : Controller
{
    private readonly IRegionService regionService;

    public RegionsController(IRegionService regionService)
    {
        this.regionService = regionService;
    }

    [HttpGet("/regions/{slug}")]
    public async Task<IActionResult> Details(string slug)
    {
        var region = await regionService.GetDetailsBySlugAsync(slug);

        if (region == null)
        {
            return NotFound();
        }

        return View(region);
    }
}