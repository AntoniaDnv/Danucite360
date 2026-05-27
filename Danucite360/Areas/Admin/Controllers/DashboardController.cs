using Danucite360.Common.Constants;
using Danucite360.Data.Data;
using Danucite360.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleConstants.Admin)]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext context;

    public DashboardController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            RegionsCount = await context.Regions.CountAsync(),
            CategoriesCount = await context.BudgetCategories.CountAsync(),
            SourcesCount = await context.BudgetSources.CountAsync(),
            BudgetRecordsCount = await context.BudgetRecords.CountAsync(),
            DemoRecordsCount = await context.BudgetRecords.CountAsync(r => r.IsDemo),
            BudgetYear = DataConstants.BudgetYear
        };

        return View(model);
    }
}