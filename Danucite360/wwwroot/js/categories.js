document.addEventListener("DOMContentLoaded", async () => {
    const canvas = document.getElementById("categoriesChart");

    if (!canvas) {
        return;
    }

    const response = await fetch("/api/dashboard/categories");

    if (!response.ok) {
        console.error("Failed to load category chart data.");
        return;
    }

    const data = await response.json();

    new Chart(canvas, {
        type: "doughnut",
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: `Spending Categories ${data.year}`,
                    data: data.values
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.label}: ${context.parsed.toLocaleString()} ${data.unit}`;
                        }
                    }
                }
            }
        }
    });
});