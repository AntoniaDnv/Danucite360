// Regional Comparison page: per-capita chart + table search.
document.addEventListener("DOMContentLoaded", () => {
    const C = window.DanuciteCharts;

    // Per-capita bar chart (data embedded in canvas data-* attributes).
    const canvas = document.getElementById("perCapitaChart");
    if (canvas && window.Chart) {
        const labels = JSON.parse(canvas.dataset.labels || "[]");
        const values = JSON.parse(canvas.dataset.values || "[]");

        new Chart(canvas, {
            type: "bar",
            data: {
                labels,
                datasets: [{
                    data: values,
                    backgroundColor: C.palette.teal,
                    borderRadius: 4,
                    maxBarThickness: 56
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: ctx => "€" + ctx.parsed.y.toLocaleString() + " / resident" } }
                },
                scales: {
                    x: { grid: { display: false } },
                    y: {
                        beginAtZero: true,
                        grid: { color: C.palette.border },
                        ticks: { callback: v => "€" + v.toLocaleString() }
                    }
                }
            }
        });
    }

    // Table search filter.
    const search = document.getElementById("regionSearch");
    const rows = Array.from(document.querySelectorAll("#regionsTable tbody tr"));
    if (search && rows.length) {
        search.addEventListener("input", () => {
            const q = search.value.trim().toLowerCase();
            rows.forEach(row => {
                const name = row.dataset.regionName || "";
                row.style.display = name.includes(q) ? "" : "none";
            });
        });
    }
});
