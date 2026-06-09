// Funding page: capital allocation donut.
document.addEventListener("DOMContentLoaded", () => {
    const C = window.DanuciteCharts;
    const canvas = document.getElementById("fundingDonut");
    if (!canvas || !window.Chart) return;

    const labels = JSON.parse(canvas.dataset.labels || "[]");
    const values = JSON.parse(canvas.dataset.values || "[]");
    const total = values.reduce((a, b) => a + Number(b), 0);

    new Chart(canvas, {
        type: "doughnut",
        data: {
            labels,
            datasets: [{
                data: values,
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
});
