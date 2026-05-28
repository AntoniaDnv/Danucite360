using System.ComponentModel.DataAnnotations;
using Danucite360.Web.ViewModels.Admin;
using Xunit;

namespace Danucite360.Tests;

public class ValidationTests
{
    [Fact]
    public void RegionInputModel_InvalidWhenNameIsMissing()
    {
        var model = new RegionInputModel
        {
            Name = "",
            Slug = "test-region",
            Population = 100000
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegionInputModel.Name)));
    }

    [Fact]
    public void RegionInputModel_InvalidWhenSlugHasInvalidCharacters()
    {
        var model = new RegionInputModel
        {
            Name = "Test Region",
            Slug = "Test Region!",
            Population = 100000
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegionInputModel.Slug)));
    }

    [Fact]
    public void RegionInputModel_InvalidWhenPopulationIsZero()
    {
        var model = new RegionInputModel
        {
            Name = "Test Region",
            Slug = "test-region",
            Population = 0
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegionInputModel.Population)));
    }

    [Fact]
    public void RegionInputModel_ValidWithCorrectData()
    {
        var model = new RegionInputModel
        {
            Name = "Test Region",
            Slug = "test-region",
            Population = 100000,
            Description = "Valid test region",
            IsDemo = true
        };

        var results = ValidateModel(model);

        Assert.Empty(results);
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        return results;
    }
}