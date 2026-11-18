using Danucite360.Common;
using Xunit;

namespace Danucite360.Tests;

public class CsvWriterTests
{
    [Fact]
    public void Write_QuotesFieldsWithCommasQuotesAndNewlines()
    {
        var csv = CsvWriter.Write(
            new[] { "name", "note" },
            new[] { new object?[] { "Пловдив, център", "say \"hi\"\nnext" } });

        Assert.Equal("name,note\r\n\"Пловдив, център\",\"say \"\"hi\"\"\nnext\"\r\n", csv);
    }

    [Fact]
    public void Write_UsesInvariantDecimalsAndEmptyForNull()
    {
        var csv = CsvWriter.Write(new[] { "amount", "spent" }, new[] { new object?[] { 1234.5m, null } });

        Assert.Equal("amount,spent\r\n1234.5,\r\n", csv);
    }
}
