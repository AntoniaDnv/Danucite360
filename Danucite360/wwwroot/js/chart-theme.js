// Shared Chart.js theme + helpers for Danucite360 (Institutional Modernism palette).
// Load this once before any page chart script.
(function () {
    const palette = {
        navy: "#1E3A5F",
        brandNavy: "#131B2E",
        teal: "#0D9488",
        blue: "#2563EB",
        amber: "#F59E0B",
        green: "#16A34A",
        red: "#DC2626",
        purple: "#7C3AED",
        cyan: "#0891B2",
        slate: "#64748B",
        slateLight: "#94A3B8",
        border: "#E2E8F0"
    };

    // Ordered series colors for categorical charts (donuts, multi-series).
    const series = [
        palette.navy,
        palette.teal,
        palette.blue,
        palette.amber,
        palette.purple,
        palette.cyan,
        palette.slate
    ];

    // Values arrive in "thousand EUR"; show citizen-friendly billions of EUR.
    function formatBillionsEur(thousandEur) {
        const billions = Number(thousandEur) / 1_000_000;
        return "€" + billions.toFixed(2) + "B";
    }

    function formatThousandEur(value) {
        return Number(value).toLocaleString(undefined, { maximumFractionDigits: 1 });
    }

    function applyDefaults() {
        if (!window.Chart) return;
        Chart.defaults.font.family =
            "'Public Sans', 'Segoe UI', system-ui, sans-serif";
        Chart.defaults.font.size = 12;
        Chart.defaults.color = palette.slate;
        Chart.defaults.plugins.legend.labels.usePointStyle = true;
        Chart.defaults.plugins.legend.labels.boxWidth = 8;
        Chart.defaults.plugins.legend.labels.padding = 16;
    }

    window.DanuciteCharts = {
        palette,
        series,
        formatBillionsEur,
        formatThousandEur,
        applyDefaults
    };

    applyDefaults();
})();
