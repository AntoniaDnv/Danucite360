using Danucite360.Common.Constants;
using Danucite360.Data.Data;
using Danucite360.Data.Models;
using Danucite360.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleConstants.Admin)]
public class BudgetRecordsController : Controller
{
    private readonly ApplicationDbContext context;

    public BudgetRecordsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IActionResult> Index()
    {
        var records = await context.BudgetRecords
            .AsNoTracking()
            .Include(r => r.Region)
            .Include(r => r.BudgetCategory)
            .Include(r => r.BudgetSource)
            .OrderBy(r => r.BudgetYear)
            .ThenBy(r => r.RecordType)
            .ThenBy(r => r.Region != null ? r.Region.Name : "")
            .ToListAsync();

        return View(records);
    }

    public async Task<IActionResult> Details(int id)
    {
        var record = await context.BudgetRecords
            .AsNoTracking()
            .Include(r => r.Region)
            .Include(r => r.BudgetCategory)
            .Include(r => r.BudgetSource)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (record == null)
        {
            return NotFound();
        }

        return View(record);
    }

    public async Task<IActionResult> Create()
    {
        var model = new BudgetRecordInputModel
        {
            BudgetYear = DataConstants.BudgetYear,
            Currency = DataConstants.DefaultCurrency,
            Unit = DataConstants.DefaultUnit,
            RecordTypes = GetRecordTypes()
        };

        await PopulateDropdownsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BudgetRecordInputModel model)
    {
        ValidateBudgetRecord(model);

        if (!ModelState.IsValid)
        {
            model.RecordTypes = GetRecordTypes();
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var record = new BudgetRecord
        {
            BudgetYear = model.BudgetYear,
            Amount = model.Amount,
            Currency = model.Currency,
            Unit = model.Unit,
            RecordType = model.RecordType,
            IsDemo = model.IsDemo,
            RegionId = model.RegionId,
            BudgetCategoryId = model.BudgetCategoryId,
            BudgetSourceId = model.BudgetSourceId,
            Notes = model.Notes
        };

        await context.BudgetRecords.AddAsync(record);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var record = await context.BudgetRecords.FindAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        var model = new BudgetRecordInputModel
        {
            BudgetYear = record.BudgetYear,
            Amount = record.Amount,
            Currency = record.Currency,
            Unit = record.Unit,
            RecordType = record.RecordType,
            IsDemo = record.IsDemo,
            RegionId = record.RegionId,
            BudgetCategoryId = record.BudgetCategoryId,
            BudgetSourceId = record.BudgetSourceId,
            Notes = record.Notes,
            RecordTypes = GetRecordTypes()
        };

        await PopulateDropdownsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BudgetRecordInputModel model)
    {
        var record = await context.BudgetRecords.FindAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        ValidateBudgetRecord(model);

        if (!ModelState.IsValid)
        {
            model.RecordTypes = GetRecordTypes();
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        record.BudgetYear = model.BudgetYear;
        record.Amount = model.Amount;
        record.Currency = model.Currency;
        record.Unit = model.Unit;
        record.RecordType = model.RecordType;
        record.IsDemo = model.IsDemo;
        record.RegionId = model.RegionId;
        record.BudgetCategoryId = model.BudgetCategoryId;
        record.BudgetSourceId = model.BudgetSourceId;
        record.Notes = model.Notes;

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var record = await context.BudgetRecords
            .AsNoTracking()
            .Include(r => r.Region)
            .Include(r => r.BudgetCategory)
            .Include(r => r.BudgetSource)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (record == null)
        {
            return NotFound();
        }

        return View(record);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var record = await context.BudgetRecords.FindAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        context.BudgetRecords.Remove(record);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(BudgetRecordInputModel model)
    {
        model.Regions = await context.Regions
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            })
            .ToListAsync();

        model.Categories = await context.BudgetCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToListAsync();

        model.Sources = await context.BudgetSources
            .AsNoTracking()
            .OrderBy(s => s.Title)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Title
            })
            .ToListAsync();
    }

    private static IEnumerable<SelectListItem> GetRecordTypes()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value = BudgetRecordTypes.NationalRevenue, Text = "National Revenue" },
            new SelectListItem { Value = BudgetRecordTypes.NationalExpense, Text = "National Expense" },
            new SelectListItem { Value = BudgetRecordTypes.BudgetBalance, Text = "Budget Balance" },
            new SelectListItem { Value = BudgetRecordTypes.RegionalExpense, Text = "Regional Expense" }
        };
    }

    private void ValidateBudgetRecord(BudgetRecordInputModel model)
    {
        if (model.RecordType == BudgetRecordTypes.RegionalExpense && model.RegionId == null)
        {
            ModelState.AddModelError(nameof(model.RegionId), "Region is required for regional expense records.");
        }

        if (model.RecordType != BudgetRecordTypes.RegionalExpense && model.RegionId != null)
        {
            ModelState.AddModelError(nameof(model.RegionId), "National records should not have a region.");
        }

        if (model.RecordType == BudgetRecordTypes.RegionalExpense && !model.IsDemo)
        {
            ModelState.AddModelError(nameof(model.IsDemo), "Regional MVP records should be marked as demo data.");
        }
    }
}