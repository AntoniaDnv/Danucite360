// Home / National Overview page charts.
document.addEventListener("DOMContentLoaded", () => {
    const C = window.DanuciteCharts;
    renderSnapshot();
    renderSpendingDonut();

    // Hero snapshot: Revenue vs Spending vs Balance (official national data).
    async function renderSnapshot() {
        const canvas = document.getElementById("budgetSnapshotChart");
        if (!canvas) return;

        const res = await fetch("/api/dashboard/national");
        if (!res.ok) {
            console.error("Failed to load national snapshot data.");
            return;
        }
        const data = await res.json();

        new Chart(canvas, {
            type: "bar",
            data: {
                labels: data.labels,
                datasets: [{
                    data: data.values,
                    backgroundColor: [C.palette.teal, C.palette.navy, C.palette.amber],
                    borderRadius: 4,
                    borderSkipped: false,
                    maxBarThickness: 48
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: ctx => C.formatBillionsEur(ctx.parsed.y)
                        }
                    }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 11 } } },
                    y: {
                        grid: { color: C.palette.border },
                        ticks: { callback: v => C.formatBillionsEur(v) }
                    }
                }
            }
        });
    }

    // "Where does it go?" — spending categories (demo regional distribution).
    async function renderSpendingDonut() {
        const canvas = document.getElementById("spendingDonut");
        if (!canvas) return;

        const res = await fetch("/api/dashboard/categories");
        if (!res.ok) {
            console.error("Failed to load category spending data.");
            return;
        }
        const data = await res.json();
        const total = data.values.reduce((a, b) => a + Number(b), 0);

        new Chart(canvas, {
            type: "doughnut",
            data: {
                labels: data.labels,
                datasets: [{
                    data: data.values,
                    backgroundColor: C.series,
                    borderColor: "#ffffff",
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: "62%",
                plugins: {
                    legend: { position: "right" },
                    tooltip: {
                        callbacks: {
                            label: ctx => {
                                const pct = total ? ((ctx.parsed / total) * 100).toFixed(1) : 0;
                                return `${ctx.label}: ${pct}%`;
                            }
                        }
                    }
                }
            }
        });
    }
});
