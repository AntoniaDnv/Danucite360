using Danucite360.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Danucite360.Web.Controllers;

public class SourcesController : Controller
{
    private readonly ISourceService sourceService;

    public SourcesController(ISourceService sourceService)
    {
        this.sourceService = sourceService;
    }

    public async Task<IActionResult> Index()
    {
        var sources = await sourceService.GetAllAsync();
        return View(sources);
    }

    public async Task<IActionResult> Details(int id)
    {
        var source = await sourceService.GetByIdAsync(id);

        if (source == null)
        {
            return NotFound();
        }

        return View(source);
    }
}