using Danucite360.Common.Constants;
using Danucite360.Data.Data;
using Danucite360.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Tests;

public static class TestDbContextFactory
{
    public static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);

        SeedTestData(context);

        return context;
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        var source = new BudgetSource
        {
            Id = 1,
            Title = "Test Source",
            Publisher = "Test Publisher",
            Url = "https://example.com",
            RetrievedOn = DateTime.UtcNow,
            Notes = "Test source"
        };

        var revenueCategory = new BudgetCategory
        {
            Id = 1,
            Name = "Revenue",
            Slug = "revenue",
            Description = "Revenue"
        };

        var expensesCategory = new BudgetCategory
        {
            Id = 2,
            Name = "Expenses",
            Slug = "expenses",
            Description = "Expenses"
        };

        var balanceCategory = new BudgetCategory
        {
            Id = 3,
            Name = "Budget Balance",
            Slug = "budget-balance",
            Description = "Budget Balance"
        };

        var educationCategory = new BudgetCategory
        {
            Id = 4,
            Name = "Education",
            Slug = "education",
            Description = "Education"
        };

        var healthcareCategory = new BudgetCategory
        {
            Id = 5,
            Name = "Healthcare",
            Slug = "healthcare",
            Description = "Healthcare"
        };

        var sofia = new Region
        {
            Id = 1,
            Name = "Sofia",
            Slug = "sofia",
            Description = "Test Sofia",
            Population = 1286000,
            IsDemo = true
        };

        var plovdiv = new Region
        {
            Id = 2,
            Name = "Plovdiv",
            Slug = "plovdiv",
            Description = "Test Plovdiv",
            Population = 634000,
            IsDemo = true
        };

        context.BudgetSources.Add(source);

        context.BudgetCategories.AddRange(
            revenueCategory,
            expensesCategory,
            balanceCategory,
            educationCategory,
            healthcareCategory);

        context.Regions.AddRange(sofia, plovdiv);

        context.BudgetRecords.AddRange(
            new BudgetRecord
            {
                Id = 1,
                BudgetYear = DataConstants.BudgetYear,
                Amount = 30369776.7m,
                Currency = "EUR",
                Unit = "thousand EUR",
                RecordType = BudgetRecordTypes.NationalRevenue,
                IsDemo = false,
                BudgetCategoryId = revenueCategory.Id,
                BudgetSourceId = source.Id
            },
            new BudgetRecord
            {
                Id = 2,
                BudgetYear = DataConstants.BudgetYear,
                Amount = 34948286.1m,
                Currency = "EUR",
                Unit = "thousand EUR",
                RecordType = BudgetRecordTypes.NationalExpense,
                IsDemo = false,
                BudgetCategoryId = expensesCategory.Id,
                BudgetSourceId = source.Id
            },
            new BudgetRecord
            {
                Id = 3,
                BudgetYear = DataConstants.BudgetYear,
                Amount = -4578509.4m,
                Currency = "EUR",
                Unit = "thousand EUR",
                RecordType = BudgetRecordTypes.BudgetBalance,
                IsDemo = false,
                BudgetCategoryId = balanceCategory.Id,
                BudgetSourceId = source.Id
            },
            new BudgetRecord
            {
                Id = 4,
                BudgetYear = DataConstants.BudgetYear,
                Amount = 120000m,
                Currency = "EUR",
                Unit = "thousand EUR",
                RecordType = BudgetRecordTypes.RegionalExpense,
                IsDemo = true,
                RegionId = sofia.Id,
                BudgetCategoryId = educationCategory.Id,
                BudgetSourceId = source.Id
            },
            new BudgetRecord
            {
                Id = 5,
                BudgetYear = DataConstants.BudgetYear,
                Amount = 95000m,
                Currency = "EUR",
                Unit = "thousand EUR",
                RecordType = BudgetRecordTypes.RegionalExpense,
                IsDemo = true,
                RegionId = sofia.Id,
                BudgetCategoryId = healthcareCategory.Id,
                BudgetSourceId = source.Id
            },
            new BudgetRecord
            {
                Id = 6,
                BudgetYear = DataConstants.BudgetYear,
                Amount = 70000m,
                Currency = "EUR",
                Unit = "thousand EUR",
                RecordType = BudgetRecordTypes.RegionalExpense,
                IsDemo = true,
                RegionId = plovdiv.Id,
                BudgetCategoryId = educationCategory.Id,
                BudgetSourceId = source.Id
            });

        context.SaveChanges();
    }
}