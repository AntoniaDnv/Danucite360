// Shared Chart.js theme + helpers for Danucite360 (Institutional Modernism palette).
// Load this once before any page chart script.
(function () {
    const palette = {
        // Civic layer: one accent (blue) leads; "teal"/"navy" now map to accent / ink so old charts follow.
        navy: "#1E3A5F",
        brandNavy: "#1E3A5F",
        teal: "#2563EB",
        blue: "#2563EB",
        amber: "#F59E0B",
        green: "#16A34A",
        red: "#DC2626",
        purple: "#7C3AED",
        cyan: "#0891B2",
        slate: "#6B7487",
        slateLight: "#9AA3B5",
        border: "#E5E7EB"
    };

    // Ordered series colors for categorical charts (donuts, multi-series).
    // Accent leads, the rest step down through ink neutrals.
    const series = ["#2563EB", "#1E3A5F", "#475569", "#64748B", "#94A3B8", "#CBD5E1", "#E2E8F0"];

    // Values arrive in "thousand EUR"; show Bulgarian-formatted billions of EUR (e.g. "30,37 млрд. €").
    function formatBillionsEur(thousandEur) {
        const billions = Number(thousandEur) / 1_000_000;
        return billions.toLocaleString("bg-BG", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + " млрд. €";
    }

    function formatThousandEur(value) {
        return Number(value).toLocaleString("bg-BG", { maximumFractionDigits: 1 });
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
