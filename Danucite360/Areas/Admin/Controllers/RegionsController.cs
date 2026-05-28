using Danucite360.Common.Constants;
using Danucite360.Data.Data;
using Danucite360.Data.Models;
using Danucite360.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleConstants.Admin)]
public class RegionsController : Controller
{
    private readonly ApplicationDbContext context;

    public RegionsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IActionResult> Index()
    {
        var regions = await context.Regions
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .ToListAsync();

        return View(regions);
    }

    public async Task<IActionResult> Details(int id)
    {
        var region = await context.Regions
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (region == null)
        {
            return NotFound();
        }

        return View(region);
    }

    public IActionResult Create()
    {
        return View(new RegionInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegionInputModel model)
    {
        if (await context.Regions.AnyAsync(r => r.Slug == model.Slug))
        {
            ModelState.AddModelError(nameof(model.Slug), "A region with this slug already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var region = new Region
        {
            Name = model.Name,
            Slug = model.Slug,
            Description = model.Description,
            Population = model.Population,
            IsDemo = model.IsDemo
        };

        await context.Regions.AddAsync(region);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var region = await context.Regions.FindAsync(id);

        if (region == null)
        {
            return NotFound();
        }

        var model = new RegionInputModel
        {
            Name = region.Name,
            Slug = region.Slug,
            Description = region.Description,
            Population = region.Population,
            IsDemo = region.IsDemo
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RegionInputModel model)
    {
        var region = await context.Regions.FindAsync(id);

        if (region == null)
        {
            return NotFound();
        }

        if (await context.Regions.AnyAsync(r => r.Slug == model.Slug && r.Id != id))
        {
            ModelState.AddModelError(nameof(model.Slug), "A region with this slug already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        region.Name = model.Name;
        region.Slug = model.Slug;
        region.Description = model.Description;
        region.Population = model.Population;
        region.IsDemo = model.IsDemo;

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var region = await context.Regions
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (region == null)
        {
            return NotFound();
        }

        return View(region);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var region = await context.Regions
            .Include(r => r.BudgetRecords)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (region == null)
        {
            return NotFound();
        }

        if (region.BudgetRecords.Any())
        {
            TempData["ErrorMessage"] = "This region cannot be deleted because it has budget records.";
            return RedirectToAction(nameof(Index));
        }

        context.Regions.Remove(region);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}