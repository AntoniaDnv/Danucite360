using Danucite360.Common.Constants;
using Danucite360.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Data.Data;

/// <summary>
/// Demo evidence trail per project: allocation → procurement → contract → payments → progress.
/// Deterministic, and consistent with the project: payments add up exactly to Spent.
/// Every row is IsDemo — these are illustrative documents, not real ones.
/// </summary>
public static partial class SeedData
{
    private const string DemoDoc = "Демонстрационен документ — не е реален";

    public static List<ProjectEvidence> BuildEvidence(FundedProject p)
    {
        var year = p.BudgetYear;
        var started = p.Status != ProjectStatuses.InPlanning;
        var list = new List<ProjectEvidence>
        {
            Item(EvidenceStages.Allocation, "Средствата са предвидени в бюджета", new DateTime(year, 1, 1), p.Budget, "Закон за държавния бюджет " + year + " — ред за проекта"),
            Item(EvidenceStages.Procurement, "Обявена обществена поръчка", new DateTime(year, 2, 16), null, "Обявление в регистъра на обществените поръчки"),
        };

        if (started)
        {
            list.Add(Item(EvidenceStages.Contract, "Подписан договор с изпълнител", new DateTime(year, 4, 10), Math.Round(p.Budget * 0.96m, 2), "Договор за изпълнение"));
        }

        var spent = p.Spent ?? 0m;
        if (spent > 0)
        {
            if (started && spent >= 1_000m)
            {
                // Two payments that add up exactly to Spent.
                var first = Math.Round(spent * 0.4m, 2);
                list.Add(Item(EvidenceStages.Payment, "Авансово плащане", new DateTime(year, 5, 20), first, "Платежно нареждане"));
                list.Add(Item(EvidenceStages.Payment, "Междинно плащане", new DateTime(year, 8, 29), spent - first, "Платежно нареждане"));
            }
            else
            {
                list.Add(Item(EvidenceStages.Payment, started ? "Плащане" : "Аванс за проектиране", new DateTime(year, 5, 20), spent, "Платежно нареждане"));
            }
        }

        list.Add(Item(EvidenceStages.Progress, $"Отчет: {p.CompletionPercent}% от работата е извършена", new DateTime(year, 9, 30), null, "Протокол за напредък"));
        return list;
    }

    private static ProjectEvidence Item(string stage, string title, DateTime date, decimal? amount, string document) => new()
    {
        Stage = stage, Title = title, OccurredOn = date, Amount = amount, DocumentTitle = document + " · " + DemoDoc, IsDemo = true
    };

    private static async Task SeedEvidenceAsync(ApplicationDbContext context)
    {
        if (await context.ProjectEvidence.AnyAsync())
        {
            return;
        }

        foreach (var p in await context.FundedProjects.ToListAsync())
        {
            foreach (var e in BuildEvidence(p))
            {
                e.FundedProjectId = p.Id;
                context.ProjectEvidence.Add(e);
            }
        }

        await context.SaveChangesAsync();
    }
}
