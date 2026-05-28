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

    new Chart(canvas, {
        type: "bar",
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: `Amount (${data.unit})`,
                    data: data.values,
                    borderWidth: 1,
                    borderRadius: 8
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
                            return `${context.parsed.y.toLocaleString()} ${data.unit}`;
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
                            return value.toLocaleString();
                        }
                    }
                }
            }
        }
    });
});