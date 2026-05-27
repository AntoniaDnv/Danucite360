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
                    label: `National Budget ${data.year} (${data.unit})`,
                    data: data.values
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    display: true
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
                y: {
                    beginAtZero: true
                }
            }
        }
    });
});