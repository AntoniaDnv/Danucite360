using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

public class MapController : Controller
{
    private readonly IRegionService regionService;

    public MapController(IRegionService regionService)
    {
        this.regionService = regionService;
    }

    public async Task<IActionResult> Index()
    {
        var regions = await regionService.GetAllAsync();
        return View(regions);
    }
}