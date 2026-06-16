// Debt & Guarantees page charts.
document.addEventListener("DOMContentLoaded", () => {
    const C = window.DanuciteCharts;
    if (!window.Chart) return;

    // Domestic vs External trend (stacked bars), thousand EUR.
    const trend = document.getElementById("debtTrendChart");
    if (trend) {
        const labels = JSON.parse(trend.dataset.labels || "[]");
        const domestic = JSON.parse(trend.dataset.domestic || "[]");
        const external = JSON.parse(trend.dataset.external || "[]");

        new Chart(trend, {
            type: "bar",
            data: {
                labels,
                datasets: [
                    { label: "Domestic", data: domestic, backgroundColor: C.palette.navy, stack: "d", borderRadius: 2, maxBarThickness: 44 },
                    { label: "External", data: external, backgroundColor: C.palette.teal, stack: "d", borderRadius: 2, maxBarThickness: 44 }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: "bottom" },
                    tooltip: { callbacks: { label: ctx => `${ctx.dataset.label}: ${C.formatBillionsEur(ctx.parsed.y)}` } }
                },
                scales: {
                    x: { stacked: true, grid: { display: false } },
                    y: { stacked: true, grid: { color: C.palette.border }, ticks: { callback: v => C.formatBillionsEur(v) } }
                }
            }
        });
    }

    // Debt structure by instrument (donut), percentages.
    const inst = document.getElementById("debtInstrumentChart");
    if (inst) {
        const labels = JSON.parse(inst.dataset.labels || "[]");
        const values = JSON.parse(inst.dataset.values || "[]");

        new Chart(inst, {
            type: "doughnut",
            data: { labels, datasets: [{ data: values, backgroundColor: C.series, borderColor: "#fff", borderWidth: 2 }] },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: "62%",
                plugins: {
                    legend: { position: "right" },
                    tooltip: { callbacks: { label: ctx => `${ctx.label}: ${ctx.parsed}%` } }
                }
            }
        });
    }

    // Yield curve (line) — yield % by maturity (years).
    const yc = document.getElementById("yieldCurveChart");
    if (yc) {
        const points = JSON.parse(yc.dataset.points || "[]");
        new Chart(yc, {
            type: "line",
            data: {
                datasets: [{
                    label: "Avg. yield",
                    data: points,
                    borderColor: C.palette.navy,
                    backgroundColor: "rgba(19,27,46,0.10)",
                    borderWidth: 2,
                    pointRadius: 5,
                    pointBackgroundColor: C.palette.navy,
                    fill: true,
                    tension: 0.3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                parsing: false,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: ctx => `${ctx.raw.x}y: ${ctx.raw.y}%` } }
                },
                scales: {
                    x: { type: "linear", title: { display: true, text: "Maturity (years)" }, grid: { color: C.palette.border } },
                    y: { title: { display: true, text: "Yield %" }, grid: { color: C.palette.border }, ticks: { callback: v => v + "%" } }
                }
            }
        });
    }
});
