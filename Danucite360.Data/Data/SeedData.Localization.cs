using Danucite360.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Data.Data;

/// <summary>
/// Bulgarian text for seeded rows, plus map coordinates / spent amounts for demo projects.
/// Runs on every startup but only touches rows whose text is still the English seed
/// (no Cyrillic) or that lack coordinates, so admin edits in Bulgarian are never overwritten.
/// </summary>
public static partial class SeedData
{
    private static bool IsCyrillic(string? s) => s != null && s.Any(c => c >= 'А' && c <= 'я');

    private static readonly Dictionary<string, (string Name, string Description)> RegionsBg = new()
    {
        ["sofia"] = ("София", "Столичен регион — част от демонстрационния набор данни на Danucite360."),
        ["plovdiv"] = ("Пловдив", "Демонстрационен регион за визуализация на регионални финанси."),
        ["varna"] = ("Варна", "Демонстрационен регион за визуализация на регионални финанси."),
        ["burgas"] = ("Бургас", "Демонстрационен регион за визуализация на регионални финанси."),
        ["ruse"] = ("Русе", "Демонстрационен регион за визуализация на регионални финанси."),
        ["stara-zagora"] = ("Стара Загора", "Демонстрационен регион за визуализация на регионални финанси."),
    };

    private static readonly Dictionary<string, (string Name, string Description)> CategoriesBg = new()
    {
        ["vat"] = ("Данък добавена стойност (ДДС)", "Данък, включен в цената на почти всички стоки и услуги."),
        ["personal-income-tax"] = ("Данък върху доходите на физическите лица", "Данъкът, който се удържа от заплатите и другите лични доходи."),
        ["corporate-tax"] = ("Корпоративен данък", "Данък върху печалбата на фирмите."),
        ["excise-duties"] = ("Акцизи", "Допълнителни данъци върху горива, тютюн, алкохол и енергия."),
        ["other-tax-revenue"] = ("Други данъчни приходи", "Други по-малки данъци."),
        ["non-tax-revenue"] = ("Неданъчни приходи, помощи и дарения", "Такси, глоби, приходи от собственост и средства от ЕС."),
        ["national-expenditure"] = ("Общи разходи", "Всички разходи и трансфери по консолидирания бюджет."),
        ["budget-balance"] = ("Бюджетно салдо", "Разликата между приходите и разходите: дефицит (−) или излишък (+)."),
        ["education"] = ("Образование", "Демо регионални разходи за образование."),
        ["healthcare"] = ("Здравеопазване", "Демо регионални разходи за здравеопазване."),
        ["infrastructure"] = ("Инфраструктура", "Демо регионални разходи за инфраструктура."),
        ["social-support"] = ("Социално подпомагане", "Демо регионални разходи за социално подпомагане."),
        ["public-administration"] = ("Държавна администрация", "Демо регионални разходи за администрация."),
        ["security"] = ("Сигурност", "Демо регионални разходи за сигурност."),
        ["environment"] = ("Околна среда", "Демо регионални разходи за околна среда."),
    };

    // Approximate city-centre coordinates (demo locations, not exact sites); Spent is illustrative.
    private static readonly Dictionary<string, (string Title, string Description, string? City, double? Lat, double? Lng, decimal Spent)> ProjectsBg = new()
    {
        ["sofia-water-grid"] = ("Реконструкция на водопроводната мрежа на София", "Модернизация на водопроводната и канализационната мрежа в централните райони за по-сигурно и ефективно водоснабдяване на столицата.", "София", 42.6977, 23.3219, 7_400m),
        ["sofia-metro-line-3"] = ("Разширение на Линия 3 на метрото в София", "Удължаване на метрото до квартали без добър транспорт — по-малко задръствания и по-кратко пътуване.", "София", 42.6650, 23.2900, 31_000m),
        ["sofia-district-heating"] = ("Модернизация на топлопреносната мрежа", "Обновяване на парното отопление за по-малко загуби на енергия и по-ниски емисии в столицата.", "София", 42.7100, 23.3600, 44_200m),
        ["sofia-ring-road-south"] = ("Рехабилитация на Южната дъга на Околовръстния път", "Ново покритие и разширяване на южния околовръстен път за по-безопасно и плавно движение.", "София", 42.6400, 23.3400, 39_500m),
        ["varna-port-expansion"] = ("Разширение на пристанище Варна — етап II", "Увеличаване на капацитета на терминалите и модернизация на товарните съоръжения.", "Варна", 43.1950, 27.8900, 28_000m),
        ["varna-medical-campus"] = ("Нов кампус на Медицинския университет", "Изграждане на модерни учебни и изследователски сгради за регионалния медицински университет.", "Варна", 43.2141, 27.9147, 2_100m),
        ["varna-hospital-digital"] = ("Дигитализация на областната болница във Варна", "Електронни здравни досиета и модерна диагностична апаратура в областната болница.", "Варна", 43.2050, 27.9300, 7_900m),
        ["plovdiv-heritage-restoration"] = ("Реставрация на Стария град в Пловдив", "Възстановяване на исторически улици и сгради за опазване на културното наследство и развитие на туризма.", "Пловдив", 42.1497, 24.7510, 8_800m),
        ["plovdiv-smart-traffic"] = ("Интелигентна система за управление на трафика в Пловдив", "Адаптивни светофари и сензори за намаляване на задръстванията в града.", "Пловдив", 42.1354, 24.7453, 300m),
        ["burgas-flood-protection"] = ("Защита от наводнения по крайбрежието на Бургас", "Изграждане на брегови защитни съоръжения и подобряване на отводняването.", "Бургас", 42.5048, 27.4626, 9_100m),
        ["burgas-vocational-center"] = ("Център за професионално обучение в Бургас", "Нов център за професионална квалификация на работната сила в региона.", "Бургас", 42.5200, 27.4500, 11_600m),
        ["ruse-danube-approach"] = ("Подходни пътища към Дунав мост при Русе", "Реконструкция на пътищата, свързващи града с Дунав мост.", "Русе", 43.8356, 25.9657, 18_700m),
        ["stara-zagora-solar-park"] = ("Соларен парк в Стара Загора", "Изграждане на голям фотоволтаичен парк в подкрепа на прехода към чиста енергия.", "Стара Загора", 42.4258, 25.6345, 3_200m),
        ["stara-zagora-hospital-wing"] = ("Ново крило на областната болница в Стара Загора", "Изграждане на ново болнично крило за повече капацитет в региона.", "Стара Загора", 42.4300, 25.6200, 19_300m),
        ["national-rail-electrification"] = ("Национална програма за електрификация на жп линиите", "Електрификация и модернизация на приоритетни железопътни направления в цялата страна.", null, null, null, 96_000m),
        ["national-broadband-expansion"] = ("Национално разширяване на високоскоростния интернет", "Високоскоростен интернет за селски и слабо обслужени райони в цялата страна.", null, null, null, 4_500m),
    };

    private static readonly Dictionary<string, string> DebtItemsBg = new()
    {
        ["Securities (International)"] = "Ценни книжа (международни пазари)",
        ["Securities (Domestic)"] = "Ценни книжа (вътрешен пазар)",
        ["Loans"] = "Заеми",
        ["Deposits"] = "Депозити",
        ["EUR (Euro)"] = "EUR (евро)",
        ["USD (Dollar)"] = "USD (щатски долар)",
        ["Fixed Rate (Predictable)"] = "Фиксирана лихва (предвидима)",
        ["Floating Rate"] = "Плаваща лихва",
        ["Energy Sector"] = "Енергетика",
        ["EU Programs"] = "Програми на ЕС",
        ["Financial"] = "Финансов сектор",
        ["Other"] = "Други",
    };

    private static async Task LocalizeAsync(ApplicationDbContext context)
    {
        foreach (var r in await context.Regions.ToListAsync())
        {
            if (!IsCyrillic(r.Name) && RegionsBg.TryGetValue(r.Slug, out var bg))
            {
                (r.Name, r.Description) = bg;
            }
        }

        foreach (var c in await context.BudgetCategories.ToListAsync())
        {
            if (!IsCyrillic(c.Name) && CategoriesBg.TryGetValue(c.Slug, out var bg))
            {
                (c.Name, c.Description) = bg;
            }
        }

        foreach (var p in await context.FundedProjects.ToListAsync())
        {
            if (!ProjectsBg.TryGetValue(p.Slug, out var bg))
            {
                continue;
            }

            if (!IsCyrillic(p.Title))
            {
                p.Title = bg.Title;
                p.Description = bg.Description;
                p.Notes = "Демонстрационен проект за прототипа. Сумите и напредъкът са илюстративни, не са официални данни.";
            }

            if (p.Latitude == null && bg.Lat != null)
            {
                (p.City, p.Latitude, p.Longitude) = (bg.City, bg.Lat, bg.Lng);
            }

            p.Spent ??= bg.Spent;
        }

        foreach (var s in await context.BudgetSources.ToListAsync())
        {
            if (s.Title == "Draft Law on the State Budget of the Republic of Bulgaria for 2026")
            {
                s.Title = "Проектозакон за държавния бюджет на Република България за 2026 г.";
                s.Publisher = "Република България / Народно събрание";
                s.Notes = "Използван за ръчно въведените национални бюджетни стойности. Регионалните стойности са демонстрационни.";
            }
        }

        foreach (var snap in await context.DebtSnapshots.ToListAsync())
        {
            if (!IsCyrillic(snap.SourceTitle))
            {
                snap.SourceTitle = "Министерство на финансите — месечен бюлетин „Държавен дълг“ (април 2026)";
                snap.Period = "април 2026";
                snap.AvgMaturity = snap.AvgMaturity.Replace("y", " г.").Replace("m", " м.");
            }
        }

        var monthsBg = new Dictionary<string, string>
        {
            ["Dec"] = "дек", ["Jan"] = "яну", ["Feb"] = "фев", ["Mar"] = "мар", ["Apr"] = "апр", ["May"] = "май",
            ["Jun"] = "юни", ["Jul"] = "юли", ["Aug"] = "авг", ["Sep"] = "сеп", ["Oct"] = "окт", ["Nov"] = "ное"
        };
        foreach (var t in await context.DebtTrendPoints.ToListAsync())
        {
            var parts = t.Month.Split(' ');
            if (parts.Length == 2 && monthsBg.TryGetValue(parts[0], out var m))
            {
                t.Month = m + " " + parts[1];
            }
        }

        foreach (var a in await context.DebtAuctions.ToListAsync())
        {
            if (!IsCyrillic(a.Type))
            {
                a.Type = "ДЦК (облигации)";
                a.Maturity = a.Maturity.Replace(" Year", " години");
            }
        }

        foreach (var d in await context.DebtBreakdownItems.ToListAsync())
        {
            if (DebtItemsBg.TryGetValue(d.Name, out var bg))
            {
                d.Name = bg;
            }
        }

        await context.SaveChangesAsync();
    }
}
