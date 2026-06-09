document.addEventListener("DOMContentLoaded", async () => {
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