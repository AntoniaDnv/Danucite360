document.addEventListener("DOMContentLoaded", async () => {
    const canvas = document.getElementById("regionChart");

    if (!canvas) {
        return;
    }

    const regionSlug = canvas.dataset.regionSlug;

    if (!regionSlug) {
        console.error("Missing region slug.");
        return;
    }

    const response = await fetch(`/api/dashboard/regions/${regionSlug}`);

    if (!response.ok) {
        console.error("Failed to load region chart data.");
        return;
    }

    const data = await response.json();

    new Chart(canvas, {
        type: "bar",
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: `Regional Spending ${data.year} (${data.unit})`,
                    data: data.values
                }
            ]
        },
        options: {
            indexAxis: "y",
            responsive: true,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.parsed.x.toLocaleString()} ${data.unit}`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    beginAtZero: true
                }
            }
        }
    });
});