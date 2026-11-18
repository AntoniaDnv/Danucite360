using Danucite360.Common;
using Xunit;

namespace Danucite360.Tests;

public class MoneyFormatterTests
{
    [Theory]
    [InlineData(30_369_776.7, "30,37 млрд. €")]
    [InlineData(95_000, "95,0 млн. €")]
    [InlineData(300, "300 хил. €")]
    [InlineData(-4_578_509.4, "-4,58 млрд. €")]
    public void FormatThousandEur_UsesBulgarianConvention(double thousandEur, string expected)
    {
        Assert.Equal(expected, MoneyFormatter.FormatThousandEur((decimal)thousandEur));
    }
}
