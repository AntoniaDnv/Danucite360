document.addEventListener("DOMContentLoaded", async () => {
    await loadCategoriesChart();
    setupCategoryFilters();
});

async function loadCategoriesChart() {
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
                    data: data.values,
                    borderWidth: 2
                }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: "bottom"
                },
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
}

function setupCategoryFilters() {
    const searchInput = document.getElementById("categorySearch");
    const sortSelect = document.getElementById("categorySort");
    const grid = document.getElementById("categoryGrid");

    if (!searchInput || !sortSelect || !grid) {
        return;
    }

    const cards = Array.from(grid.querySelectorAll(".category-info-card"));

    function applyFilters() {
        const query = searchInput.value.trim().toLowerCase();
        const sortBy = sortSelect.value;

        cards.forEach(card => {
            const name = card.dataset.categoryName || "";
            card.style.display = name.includes(query) ? "grid" : "none";
        });

        const visibleCards = cards.filter(card => card.style.display !== "none");

        visibleCards.sort((a, b) => {
            if (sortBy === "name") {
                return (a.dataset.categoryName || "").localeCompare(b.dataset.categoryName || "");
            }

            if (sortBy === "percentage") {
                return Number(b.dataset.categoryPercentage) - Number(a.dataset.categoryPercentage);
            }

            return Number(b.dataset.categoryAmount) - Number(a.dataset.categoryAmount);
        });

        visibleCards.forEach(card => grid.appendChild(card));
    }

    searchInput.addEventListener("input", applyFilters);
    sortSelect.addEventListener("change", applyFilters);
}