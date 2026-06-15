document.addEventListener("DOMContentLoaded", async () => {
    renderExecutionTrajectory();

    const canvas = document.getElementById("nationalBudgetChart");

    if (!canvas) {
        return;
    }

    const response = await fetch("/api/dashboard/national");

    if (!response.ok) {
        console.error("Failed to load national budget chart data.");
        return;
    }

    const data = await response.json();
    const C = window.DanuciteCharts;

    new Chart(canvas, {
        type: "bar",
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: `Amount (${data.unit})`,
                    data: data.values,
                    backgroundColor: C ? [C.palette.teal, C.palette.navy, C.palette.amber] : undefined,
                    borderWidth: 0,
                    borderRadius: 4
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return C ? C.formatBillionsEur(context.parsed.y)
                                     : `${context.parsed.y.toLocaleString()} ${data.unit}`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    }
                },
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return C ? C.formatBillionsEur(value) : value.toLocaleString();
                        }
                    }
                }
            }
        }
    });
});

// Cumulative revenue execution trajectory (Consolidated Fiscal Programme).
function renderExecutionTrajectory() {
    const C = window.DanuciteCharts;
    const canvas = document.getElementById("executionChart");
    if (!canvas || !window.Chart) return;

    const labels = JSON.parse(canvas.dataset.labels || "[]");
    const values = JSON.parse(canvas.dataset.values || "[]");

    new Chart(canvas, {
        type: "line",
        data: {
            labels,
            datasets: [{
                label: "Cumulative revenue (YTD)",
                data: values,
                borderColor: C ? C.palette.teal : "#0D9488",
                backgroundColor: "rgba(13, 148, 136, 0.12)",
                borderWidth: 2,
                pointRadius: 4,
                pointBackgroundColor: C ? C.palette.teal : "#0D9488",
                fill: true,
                tension: 0.25
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: ctx => C ? C.formatBillionsEur(ctx.parsed.y) : ctx.parsed.y.toLocaleString()
                    }
                }
            },
            scales: {
                x: { grid: { display: false } },
                y: {
                    beginAtZero: true,
                    grid: { color: C ? C.palette.border : "#E2E8F0" },
                    ticks: { callback: v => C ? C.formatBillionsEur(v) : v.toLocaleString() }
                }
            }
        }
    });
}