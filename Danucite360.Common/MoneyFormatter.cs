using System.Globalization;

namespace Danucite360.Common;

/// <summary>
/// Formats monetary values that are stored in "thousand EUR" into citizen-friendly
/// strings (e.g. €30.37B, €555.0M, €120K) following the design system number rules.
/// </summary>
public static class MoneyFormatter
{
    public static string FormatThousandEur(decimal thousandEur)
    {
        var abs = Math.Abs(thousandEur);

        // 1,000,000 thousand EUR == €1B; 1,000 thousand EUR == €1M.
        if (abs >= 1_000_000m)
        {
            return "€" + (thousandEur / 1_000_000m).ToString("N2", CultureInfo.InvariantCulture) + "B";
        }

        if (abs >= 1_000m)
        {
            return "€" + (thousandEur / 1_000m).ToString("N1", CultureInfo.InvariantCulture) + "M";
        }

        return "€" + thousandEur.ToString("N0", CultureInfo.InvariantCulture) + "K";
    }
}
