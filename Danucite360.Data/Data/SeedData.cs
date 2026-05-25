using Danucite360.Common.Constants;
using Danucite360.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Data.Data;

public static class SeedData
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager);
        await SeedSourcesAsync(context);
        await SeedCategoriesAsync(context);
        await SeedRegionsAsync(context);
        await SeedBudgetRecordsAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(RoleConstants.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleConstants.Admin));
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@danucite360.local";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not create admin user: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, RoleConstants.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
        }
    }

    private static async Task SeedSourcesAsync(ApplicationDbContext context)
    {
        if (await context.BudgetSources.AnyAsync())
        {
            return;
        }

        context.BudgetSources.Add(new BudgetSource
        {
            Title = "Draft Law on the State Budget of the Republic of Bulgaria for 2026",
            Publisher = "Republic of Bulgaria / National Assembly",
            Url = "Uploaded PDF: 3 PZDBRB 2026 - 05.12.2025(1).pdf",
            RetrievedOn = DateTime.UtcNow,
            Notes = "Used for manually seeded national budget values. Regional values are demo data for prototype purposes."
        });

        await context.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.BudgetCategories.AnyAsync())
        {
            return;
        }

        var categories = new[]
        {
            new BudgetCategory
            {
                Name = "Revenue",
                Slug = "revenue",
                Description = "National budget revenue, aid and donations."
            },
            new BudgetCategory
            {
                Name = "Expenses",
                Slug = "expenses",
                Description = "National budget expenses."
            },
            new BudgetCategory
            {
                Name = "Budget Balance",
                Slug = "budget-balance",
                Description = "Budget surplus or deficit."
            },
            new BudgetCategory
            {
                Name = "Education",
                Slug = "education",
                Description = "Demo regional spending for education."
            },
            new BudgetCategory
            {
                Name = "Healthcare",
                Slug = "healthcare",
                Description = "Demo regional spending for healthcare."
            },
            new BudgetCategory
            {
                Name = "Infrastructure",
                Slug = "infrastructure",
                Description = "Demo regional spending for infrastructure."
            },
            new BudgetCategory
            {
                Name = "Social Support",
                Slug = "social-support",
                Description = "Demo regional spending for social support."
            },
            new BudgetCategory
            {
                Name = "Public Administration",
                Slug = "public-administration",
                Description = "Demo regional spending for public administration."
            },
            new BudgetCategory
            {
                Name = "Security",
                Slug = "security",
                Description = "Demo regional spending for security."
            },
            new BudgetCategory
            {
                Name = "Environment",
                Slug = "environment",
                Description = "Demo regional spending for environment."
            }
        };

        await context.BudgetCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRegionsAsync(ApplicationDbContext context)
    {
        if (await context.Regions.AnyAsync())
        {
            return;
        }

        var regions = new[]
        {
            new Region
            {
                Name = "Sofia",
                Slug = "sofia",
                Population = 1286000,
                Description = "Capital city district used as part of the Danucite360 demo dataset.",
                IsDemo = true
            },
            new Region
            {
                Name = "Plovdiv",
                Slug = "plovdiv",
                Population = 634000,
                Description = "Demo district for prototype regional finance visualization.",
                IsDemo = true
            },
            new Region
            {
                Name = "Varna",
                Slug = "varna",
                Population = 470000,
                Description = "Demo district for prototype regional finance visualization.",
                IsDemo = true
            },
            new Region
            {
                Name = "Burgas",
                Slug = "burgas",
                Population = 410000,
                Description = "Demo district for prototype regional finance visualization.",
                IsDemo = true
            },
            new Region
            {
                Name = "Ruse",
                Slug = "ruse",
                Population = 215000,
                Description = "Demo district for prototype regional finance visualization.",
                IsDemo = true
            },
            new Region
            {
                Name = "Stara Zagora",
                Slug = "stara-zagora",
                Population = 315000,
                Description = "Demo district for prototype regional finance visualization.",
                IsDemo = true
            }
        };

        await context.Regions.AddRangeAsync(regions);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBudgetRecordsAsync(ApplicationDbContext context)
    {
        if (await context.BudgetRecords.AnyAsync())
        {
            return;
        }

        var source = await context.BudgetSources.FirstAsync();

        var categories = await context.BudgetCategories.ToDictionaryAsync(c => c.Slug);
        var regions = await context.Regions.ToDictionaryAsync(r => r.Slug);

        var records = new List<BudgetRecord>
        {
            new BudgetRecord
            {
                BudgetYear = DataConstants.BudgetYear,
                Amount = 30369776.7m,
                Currency = DataConstants.DefaultCurrency,
                Unit = DataConstants.DefaultUnit,
                RecordType = BudgetRecordTypes.NationalRevenue,
                IsDemo = false,
                BudgetCategoryId = categories["revenue"].Id,
                BudgetSourceId = source.Id,
                Notes = "I. ПРИХОДИ, ПОМОЩИ И ДАРЕНИЯ"
            },
            new BudgetRecord
            {
                BudgetYear = DataConstants.BudgetYear,
                Amount = 17256463.0m,
                Currency = DataConstants.DefaultCurrency,
                Unit = DataConstants.DefaultUnit,
                RecordType = BudgetRecordTypes.NationalExpense,
                IsDemo = false,
                BudgetCategoryId = categories["expenses"].Id,
                BudgetSourceId = source.Id,
                Notes = "II. РАЗХОДИ"
            },
            new BudgetRecord
            {
                BudgetYear = DataConstants.BudgetYear,
                Amount = -4578509.4m,
                Currency = DataConstants.DefaultCurrency,
                Unit = DataConstants.DefaultUnit,
                RecordType = BudgetRecordTypes.BudgetBalance,
                IsDemo = false,
                BudgetCategoryId = categories["budget-balance"].Id,
                BudgetSourceId = source.Id,
                Notes = "V. БЮДЖЕТНО САЛДО"
            }
        };

        AddRegionalDemoRecords(records, regions["sofia"], categories, source.Id,
            120000, 95000, 150000, 70000, 50000, 40000, 30000);

        AddRegionalDemoRecords(records, regions["plovdiv"], categories, source.Id,
            70000, 55000, 82000, 43000, 30000, 25000, 18000);

        AddRegionalDemoRecords(records, regions["varna"], categories, source.Id,
            65000, 60000, 78000, 41000, 28000, 24000, 22000);

        AddRegionalDemoRecords(records, regions["burgas"], categories, source.Id,
            54000, 47000, 69000, 36000, 25000, 21000, 19000);

        AddRegionalDemoRecords(records, regions["ruse"], categories, source.Id,
            36000, 31000, 42000, 24000, 17000, 13000, 11000);

        AddRegionalDemoRecords(records, regions["stara-zagora"], categories, source.Id,
            43000, 39000, 52000, 28000, 20000, 16000, 14000);

        await context.BudgetRecords.AddRangeAsync(records);
        await context.SaveChangesAsync();
    }

    private static void AddRegionalDemoRecords(
        List<BudgetRecord> records,
        Region region,
        Dictionary<string, BudgetCategory> categories,
        int sourceId,
        decimal education,
        decimal healthcare,
        decimal infrastructure,
        decimal socialSupport,
        decimal publicAdministration,
        decimal security,
        decimal environment)
    {
        var values = new Dictionary<string, decimal>
        {
            ["education"] = education,
            ["healthcare"] = healthcare,
            ["infrastructure"] = infrastructure,
            ["social-support"] = socialSupport,
            ["public-administration"] = publicAdministration,
            ["security"] = security,
            ["environment"] = environment
        };

        foreach (var value in values)
        {
            records.Add(new BudgetRecord
            {
                BudgetYear = DataConstants.BudgetYear,
                Amount = value.Value,
                Currency = DataConstants.DefaultCurrency,
                Unit = DataConstants.DefaultUnit,
                RecordType = BudgetRecordTypes.RegionalExpense,
                IsDemo = true,
                RegionId = region.Id,
                BudgetCategoryId = categories[value.Key].Id,
                BudgetSourceId = sourceId,
                Notes = DataConstants.DemoDatasetNotice
            });
        }
    }
}