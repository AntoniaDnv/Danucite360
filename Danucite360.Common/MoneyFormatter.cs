using System.Globalization;

namespace Danucite360.Common;

/// <summary>
/// Formats monetary values stored in "thousand EUR" into citizen-friendly Bulgarian strings
/// (e.g. 30,37 млрд. €, 95,0 млн. €, 300 хил. €).
/// </summary>
public static class MoneyFormatter
{
    private static readonly NumberFormatInfo Bg = new()
    {
        NumberDecimalSeparator = ",",
        NumberGroupSeparator = " ",
        NegativeSign = "-"
    };

    public static string FormatThousandEur(decimal thousandEur)
    {
        var abs = Math.Abs(thousandEur);

        // 1,000,000 thousand EUR == 1 billion EUR; 1,000 thousand EUR == 1 million EUR.
        if (abs >= 1_000_000m)
        {
            return (thousandEur / 1_000_000m).ToString("N2", Bg) + " млрд. €";
        }

        if (abs >= 1_000m)
        {
            return (thousandEur / 1_000m).ToString("N1", Bg) + " млн. €";
        }

        return thousandEur.ToString("N0", Bg) + " хил. €";
    }

    public static string FormatPercent(decimal value, int decimals = 1) =>
        value.ToString("N" + decimals, Bg) + "%";
}
