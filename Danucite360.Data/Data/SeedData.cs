using Danucite360.Common.Constants;
using Danucite360.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Data.Data;

public static partial class SeedData
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        string? adminPassword = null)
    {
        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager, adminPassword);
        await SeedSourcesAsync(context);
        await SeedCategoriesAsync(context);
        await SeedRegionsAsync(context);
        await SeedBudgetRecordsAsync(context);
        await SeedProjectsAsync(context);
        await SeedDebtAsync(context);
        await SeedMeasuresAsync(context);
        await LocalizeAsync(context);
        await SeedEvidenceAsync(context); // after LocalizeAsync, which sets Spent
    }

    private static async Task SeedMeasuresAsync(ApplicationDbContext context)
    {
        if (await context.BudgetMeasures.AnyAsync())
        {
            return;
        }

        const string demoSource = "Демонстрационен запис — илюстративна мярка за прототипа, не е официален документ";

        BudgetMeasure M(string slug, string title, string category, string audiences, string ages,
            decimal? impact, string status, DateTime? effective, string summary, string means) => new()
        {
            Slug = slug, Title = title, Category = category, Audiences = audiences, AgeGroups = ages,
            FiscalImpact = impact, Unit = DataConstants.DefaultUnit, Status = status, EffectiveDate = effective,
            Summary = summary, WhatItMeans = means, BudgetYear = DataConstants.BudgetYear,
            IsDemo = true, SourceTitle = demoSource
        };

        context.BudgetMeasures.AddRange(
            M("danachno-oblekchenie-za-deca", "Данъчно облекчение за деца", "Данъци",
                "parent;employee;self-employed", "under30;30-50;50-65", -120_000m, MeasureStatuses.Proposed, new DateTime(2026, 1, 1),
                "Родителите могат да намалят облагаемия си доход за всяко ненавършило пълнолетие дете.",
                "Ако работите и отглеждате деца, ще плащате по-малко данък общ доход. За бюджета това е по-малко приходи."),
            M("cena-na-vinetka", "Промяна в цената на годишната винетка", "Такси",
                "driver;employer", "all", 35_000m, MeasureStatuses.Proposed, new DateTime(2026, 3, 1),
                "Цената на годишната електронна винетка за леки автомобили се актуализира.",
                "Ако шофирате по републиканската пътна мрежа, годишната винетка ще ви струва повече. Приходите отиват за поддръжка на пътищата."),
            M("indeksacia-na-pensii", "Индексация на пенсиите от 1 юли", "Социални плащания",
                "pensioner", "over65;50-65", -450_000m, MeasureStatuses.Adopted, new DateTime(2026, 7, 1),
                "Всички пенсии се увеличават с процент, определен по швейцарското правило.",
                "Ако получавате пенсия, тя ще нарасне от юли. За бюджета това е допълнителен разход."),
            M("minimalna-rabotna-zaplata", "Увеличение на минималната работна заплата", "Труд и доходи",
                "employee;employer", "under30;30-50;50-65", 60_000m, MeasureStatuses.InForce, new DateTime(2026, 1, 1),
                "Минималната месечна работна заплата се повишава.",
                "Ако работите на минимална заплата, ще получавате повече. Работодателите плащат повече осигуровки, което увеличава приходите."),
            M("osiguritelen-dohod-samoosiguryavashti", "Минимален осигурителен доход за самоосигуряващи се", "Осигуровки",
                "self-employed", "under30;30-50;50-65", 25_000m, MeasureStatuses.Proposed, new DateTime(2026, 1, 1),
                "Минималният месечен осигурителен доход за свободни професии и самоосигуряващи се се увеличава.",
                "Ако сте на свободна практика, минималните ви месечни осигуровки се повишават."),
            M("pomosht-za-parvoklasnici", "Еднократна помощ за първокласници", "Семейство",
                "parent", "under30;30-50", -18_000m, MeasureStatuses.InForce, new DateTime(2026, 9, 1),
                "Семействата получават еднократна помощ при записване на дете в първи клас.",
                "Ако детето ви тръгва на училище, можете да подадете заявление за помощ в дирекция „Социално подпомагане“."),
            M("mladezhka-zaetost", "Насърчаване на заетостта на младежи", "Труд и доходи",
                "employee;employer", "under30", -15_000m, MeasureStatuses.Proposed, new DateTime(2026, 4, 1),
                "Работодателите, наемащи младежи до 29 г. без опит, получават частично покриване на осигуровките.",
                "Ако сте млад човек, търсещ първа работа, работодателите имат стимул да ви наемат."),
            M("evro-zakrugliane", "Правила за закръгляне на цени след въвеждане на еврото", "Потребители",
                "everyone", "all", null, MeasureStatuses.InForce, new DateTime(2026, 1, 1),
                "Цените се преизчисляват по фиксирания курс 1,95583 лв. за 1 евро и се закръглят до цент.",
                "Засяга всички граждани. Мярката няма пряк ефект върху бюджета, а защитава потребителите при смяната на валутата."),
            M("dds-restoranti", "ДДС ставка за ресторантьорски услуги", "Данъци",
                "employer;everyone", "all", 90_000m, MeasureStatuses.Proposed, new DateTime(2026, 1, 1),
                "Намалената ставка на ДДС за ресторанти се връща към стандартните 20%.",
                "Храненето навън може да поскъпне. За бюджета това означава повече приходи от ДДС."));

        await context.SaveChangesAsync();
    }

    private static async Task SeedDebtAsync(ApplicationDbContext context)
    {
        if (await context.DebtSnapshots.AnyAsync())
        {
            return;
        }

        context.DebtSnapshots.Add(CreateDebtSnapshot());
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Builds the April 2026 government debt snapshot from the Ministry of Finance
    /// Government Debt monthly bulletin. Shared by production seeding and tests.
    /// </summary>
    public static DebtSnapshot CreateDebtSnapshot()
    {
        return new DebtSnapshot
        {
            Period = "April 2026",
            BudgetYear = DataConstants.BudgetYear,
            AsOf = new DateTime(2026, 4, 30),
            SourceTitle = "Ministry of Finance — Government Debt monthly bulletin (April 2026)",
            Unit = DataConstants.DefaultUnit,

            TotalDebt = 33_752_300m,
            DebtToGdpPercent = 26.9m,
            DomesticDebt = 8_477_800m,
            DomesticSharePercent = 25.1m,
            ExternalDebt = 25_274_500m,
            ExternalSharePercent = 74.9m,
            GuaranteedDebt = 637_400m,
            GuaranteedToGdpPercent = 0.5m,
            StateDebt = 31_750_000m,
            StateDebtSharePercent = 94.1m,
            EurDenominatedPercent = 96.2m,
            FixedRatePercent = 99.8m,
            AvgInterestPercent = 3.2m,
            AvgMaturity = "8y 5m",

            Breakdowns = new List<DebtBreakdownItem>
            {
                new() { Section = "Instrument", Name = "Securities (International)", Percent = 67.1m, SortOrder = 0 },
                new() { Section = "Instrument", Name = "Securities (Domestic)", Percent = 18.6m, SortOrder = 1 },
                new() { Section = "Instrument", Name = "Loans", Percent = 8.8m, SortOrder = 2 },
                new() { Section = "Instrument", Name = "Deposits", Percent = 5.5m, SortOrder = 3 },
                new() { Section = "Currency", Name = "EUR (Euro)", Percent = 96.2m, SortOrder = 0 },
                new() { Section = "Currency", Name = "USD (Dollar)", Percent = 3.8m, SortOrder = 1 },
                new() { Section = "Interest", Name = "Fixed Rate (Predictable)", Percent = 99.8m, SortOrder = 0 },
                new() { Section = "Interest", Name = "Floating Rate", Percent = 0.2m, SortOrder = 1 },
                new() { Section = "Guaranteed", Name = "Energy Sector", Percent = 51.8m, SortOrder = 0 },
                new() { Section = "Guaranteed", Name = "EU Programs", Percent = 24.5m, SortOrder = 1 },
                new() { Section = "Guaranteed", Name = "Financial", Percent = 19.6m, SortOrder = 2 },
                new() { Section = "Guaranteed", Name = "Other", Percent = 4.1m, SortOrder = 3 }
            },

            TrendPoints = new List<DebtTrendPoint>
            {
                new() { Month = "Dec 25", Domestic = 8_747_400m, External = 24_061_700m, SortOrder = 0 },
                new() { Month = "Jan 26", Domestic = 8_440_400m, External = 24_711_800m, SortOrder = 1 },
                new() { Month = "Feb 26", Domestic = 7_877_100m, External = 25_599_900m, SortOrder = 2 },
                new() { Month = "Mar 26", Domestic = 8_177_400m, External = 25_538_400m, SortOrder = 3 },
                new() { Month = "Apr 26", Domestic = 8_477_800m, External = 25_274_500m, SortOrder = 4 }
            },

            Auctions = new List<DebtAuction>
            {
                new() { Date = "06.04.2026", Type = "GS (Bonds)", Maturity = "10 Year", YieldPercent = 4.18m, SortOrder = 0 },
                new() { Date = "20.04.2026", Type = "GS (Bonds)", Maturity = "5 Year", YieldPercent = 3.33m, SortOrder = 1 }
            }
        };
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(RoleConstants.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleConstants.Admin));
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, string? adminPassword)
    {
        const string adminEmail = "admin@danucite360.local";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            // The password comes from configuration (Seed:AdminPassword), never from source control.
            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }

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