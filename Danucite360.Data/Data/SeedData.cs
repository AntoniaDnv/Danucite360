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
        await SeedProjectsAsync(context);
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
            // National revenue streams (from the 2026 draft State Budget).
            new BudgetCategory
            {
                Name = "Value Added Tax (VAT)",
                Slug = "vat",
                Description = "Revenue from value added tax."
            },
            new BudgetCategory
            {
                Name = "Personal Income Tax",
                Slug = "personal-income-tax",
                Description = "Revenue from personal income tax."
            },
            new BudgetCategory
            {
                Name = "Corporate Tax",
                Slug = "corporate-tax",
                Description = "Revenue from corporate income tax."
            },
            new BudgetCategory
            {
                Name = "Excise Duties",
                Slug = "excise-duties",
                Description = "Revenue from excise duties."
            },
            new BudgetCategory
            {
                Name = "Other Tax Revenue",
                Slug = "other-tax-revenue",
                Description = "Remaining tax revenue streams."
            },
            new BudgetCategory
            {
                Name = "Non-tax Revenue, Aid & Donations",
                Slug = "non-tax-revenue",
                Description = "Non-tax revenue, aid and donations."
            },
            new BudgetCategory
            {
                Name = "National Expenditure",
                Slug = "national-expenditure",
                Description = "Total national budget expenditure and transfers."
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

        // National revenue streams — sum to total revenue of 30,369,776.7 thousand EUR.
        var revenueStreams = new (string Slug, decimal Amount, string Notes)[]
        {
            ("vat", 14_385_215.2m, "Value Added Tax (VAT)"),
            ("personal-income-tax", 4_731_244.3m, "Personal income tax"),
            ("corporate-tax", 3_653_304.7m, "Corporate income tax"),
            ("excise-duties", 4_242_028.8m, "Excise duties"),
            ("other-tax-revenue", 501_058.6m, "Other tax revenue"),
            ("non-tax-revenue", 2_856_925.1m, "Non-tax revenue, aid and donations")
        };

        var records = new List<BudgetRecord>();

        foreach (var stream in revenueStreams)
        {
            records.Add(new BudgetRecord
            {
                BudgetYear = DataConstants.BudgetYear,
                Amount = stream.Amount,
                Currency = DataConstants.DefaultCurrency,
                Unit = DataConstants.DefaultUnit,
                RecordType = BudgetRecordTypes.NationalRevenue,
                IsDemo = false,
                BudgetCategoryId = categories[stream.Slug].Id,
                BudgetSourceId = source.Id,
                Notes = stream.Notes
            });
        }

        // Total national expenditure (= revenue - balance), so the figures reconcile.
        records.Add(new BudgetRecord
        {
            BudgetYear = DataConstants.BudgetYear,
            Amount = 34_948_286.1m,
            Currency = DataConstants.DefaultCurrency,
            Unit = DataConstants.DefaultUnit,
            RecordType = BudgetRecordTypes.NationalExpense,
            IsDemo = false,
            BudgetCategoryId = categories["national-expenditure"].Id,
            BudgetSourceId = source.Id,
            Notes = "Total expenditure and transfers"
        });

        records.Add(new BudgetRecord
        {
            BudgetYear = DataConstants.BudgetYear,
            Amount = -4_578_509.4m,
            Currency = DataConstants.DefaultCurrency,
            Unit = DataConstants.DefaultUnit,
            RecordType = BudgetRecordTypes.BudgetBalance,
            IsDemo = false,
            BudgetCategoryId = categories["budget-balance"].Id,
            BudgetSourceId = source.Id,
            Notes = "Budget balance (deficit)"
        });

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

    private static async Task SeedProjectsAsync(ApplicationDbContext context)
    {
        if (await context.FundedProjects.AnyAsync())
        {
            return;
        }

        var source = await context.BudgetSources.FirstAsync();
        var regions = await context.Regions.ToDictionaryAsync(r => r.Slug);

        var projects = new List<FundedProject>();

        void Add(string title, string slug, string type, string status, int completion,
            string? regionSlug, string description, params (string Source, decimal Amount)[] funding)
        {
            var budget = funding.Sum(f => f.Amount);

            projects.Add(new FundedProject
            {
                Title = title,
                Slug = slug,
                ProjectType = type,
                Status = status,
                CompletionPercent = completion,
                Budget = budget,
                Currency = DataConstants.DefaultCurrency,
                Unit = DataConstants.DefaultUnit,
                BudgetYear = DataConstants.BudgetYear,
                IsDemo = true,
                Notes = DataConstants.DemoDatasetNotice,
                RegionId = regionSlug == null ? null : regions[regionSlug].Id,
                BudgetSourceId = source.Id,
                Description = description,
                FundingShares = funding
                    .Select(f => new ProjectFunding { SourceName = f.Source, Amount = f.Amount })
                    .ToList()
            });
        }

        Add("Reconstruction of the Sofia Municipality Water Grid", "sofia-water-grid",
            ProjectTypes.Infrastructure, ProjectStatuses.Ongoing, 65, "sofia",
            "Modernization of water supply and sewerage networks across central districts to improve water security and efficiency for the capital region.",
            (FundingSources.EuCohesionFund, 8000m), (FundingSources.NationalBudget, 3000m), (FundingSources.Municipality, 1500m));

        Add("Extension of Sofia Metro Line 3", "sofia-metro-line-3",
            ProjectTypes.Infrastructure, ProjectStatuses.Ongoing, 40, "sofia",
            "Extension of the metro network to underserved neighbourhoods, reducing congestion and commute times.",
            (FundingSources.EuCohesionFund, 60000m), (FundingSources.NationalBudget, 25000m), (FundingSources.Municipality, 10000m));

        Add("Modernization of District Heating Network", "sofia-district-heating",
            ProjectTypes.Energy, ProjectStatuses.Completed, 100, "sofia",
            "Upgrade of the district heating system to cut energy losses and emissions across the capital.",
            (FundingSources.EuRecoveryPlan, 30000m), (FundingSources.NationalBudget, 15000m));

        Add("Rehabilitation of Ring Road South Arc", "sofia-ring-road-south",
            ProjectTypes.Infrastructure, ProjectStatuses.Ongoing, 55, "sofia",
            "Resurfacing and widening of the southern ring road to improve safety and traffic flow.",
            (FundingSources.EuCohesionFund, 50000m), (FundingSources.NationalBudget, 30000m));

        Add("Port of Varna Expansion Phase II", "varna-port-expansion",
            ProjectTypes.Infrastructure, ProjectStatuses.Ongoing, 30, "varna",
            "Expansion of port terminal capacity and modernization of cargo handling facilities.",
            (FundingSources.EuCohesionFund, 70000m), (FundingSources.NationalBudget, 35000m), (FundingSources.Municipality, 15000m));

        Add("Medical University Campus Upgrade", "varna-medical-campus",
            ProjectTypes.Education, ProjectStatuses.InPlanning, 10, "varna",
            "Construction of modern teaching and research facilities for the regional medical university.",
            (FundingSources.EuRecoveryPlan, 30000m), (FundingSources.NationalBudget, 15000m));

        Add("Varna Regional Hospital Digitalization", "varna-hospital-digital",
            ProjectTypes.Healthcare, ProjectStatuses.Ongoing, 50, "varna",
            "Rollout of electronic health records and modern diagnostic equipment across the regional hospital.",
            (FundingSources.EuRecoveryPlan, 12000m), (FundingSources.NationalBudget, 6000m));

        Add("Plovdiv Old Town Heritage Restoration", "plovdiv-heritage-restoration",
            ProjectTypes.Infrastructure, ProjectStatuses.Ongoing, 45, "plovdiv",
            "Restoration of historic streets and buildings to preserve cultural heritage and support tourism.",
            (FundingSources.EuCohesionFund, 14000m), (FundingSources.NationalBudget, 5000m), (FundingSources.Municipality, 3000m));

        Add("Plovdiv Smart Traffic System", "plovdiv-smart-traffic",
            ProjectTypes.Digitalization, ProjectStatuses.InPlanning, 5, "plovdiv",
            "Deployment of adaptive traffic signals and sensors to reduce congestion across the city.",
            (FundingSources.EuRecoveryPlan, 6000m), (FundingSources.Municipality, 3000m));

        Add("Burgas Coastal Flood Protection", "burgas-flood-protection",
            ProjectTypes.Environment, ProjectStatuses.Ongoing, 35, "burgas",
            "Construction of coastal defences and drainage upgrades to protect against flooding.",
            (FundingSources.EuCohesionFund, 20000m), (FundingSources.NationalBudget, 8000m));

        Add("Burgas Vocational Education Center", "burgas-vocational-center",
            ProjectTypes.Education, ProjectStatuses.Completed, 100, "burgas",
            "A new vocational training centre delivering skills programmes for the regional workforce.",
            (FundingSources.EuRecoveryPlan, 8000m), (FundingSources.NationalBudget, 4000m));

        Add("Ruse Danube Bridge Approach Roads", "ruse-danube-approach",
            ProjectTypes.Infrastructure, ProjectStatuses.Ongoing, 60, "ruse",
            "Reconstruction of approach roads connecting the city to the Danube bridge crossing.",
            (FundingSources.EuCohesionFund, 22000m), (FundingSources.NationalBudget, 12000m));

        Add("Stara Zagora Solar Energy Park", "stara-zagora-solar-park",
            ProjectTypes.Energy, ProjectStatuses.InPlanning, 15, "stara-zagora",
            "Development of a utility-scale solar park to support the region's clean energy transition.",
            (FundingSources.EuRecoveryPlan, 28000m), (FundingSources.NationalBudget, 12000m));

        Add("Stara Zagora Regional Hospital Wing", "stara-zagora-hospital-wing",
            ProjectTypes.Healthcare, ProjectStatuses.Ongoing, 70, "stara-zagora",
            "Construction of a new hospital wing expanding regional clinical capacity.",
            (FundingSources.EuCohesionFund, 16000m), (FundingSources.NationalBudget, 10000m));

        Add("National Rail Electrification Programme", "national-rail-electrification",
            ProjectTypes.Infrastructure, ProjectStatuses.Ongoing, 25, null,
            "Nationwide electrification and modernization of priority rail corridors.",
            (FundingSources.EuCohesionFund, 300000m), (FundingSources.NationalBudget, 120000m));

        Add("National Broadband Expansion", "national-broadband-expansion",
            ProjectTypes.Digitalization, ProjectStatuses.InPlanning, 8, null,
            "Expansion of high-speed broadband to rural and underserved communities nationwide.",
            (FundingSources.EuRecoveryPlan, 60000m), (FundingSources.NationalBudget, 25000m));

        await context.FundedProjects.AddRangeAsync(projects);
        await context.SaveChangesAsync();
    }
}