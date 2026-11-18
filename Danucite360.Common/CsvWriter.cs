using System.Globalization;
using System.Text;

namespace Danucite360.Common;

/// <summary>RFC 4180 CSV: comma-separated, CRLF rows, invariant numbers so the file is machine-readable.</summary>
public static class CsvWriter
{
    public static string Write(IEnumerable<string> headers, IEnumerable<object?[]> rows)
    {
        var sb = new StringBuilder();
        sb.Append(string.Join(",", headers.Select(Escape))).Append("\r\n");

        foreach (var row in rows)
        {
            sb.Append(string.Join(",", row.Select(v => Escape(Convert.ToString(v, CultureInfo.InvariantCulture))))).Append("\r\n");
        }

        return sb.ToString();
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0
            ? "\"" + value.Replace("\"", "\"\"") + "\""
            : value;
    }
}
