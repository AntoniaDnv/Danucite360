document.addEventListener("DOMContentLoaded", async () => {
    await loadCategoriesChart();
    setupCategorySearchAndSort();
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

function setupCategorySearchAndSort() {
    const searchInput = document.getElementById("categorySearch");
    const sortSelect = document.getElementById("categorySort");
    const grid = document.getElementById("categoryGrid");

    if (!searchInput || !sortSelect || !grid) {
        console.warn("Category search/sort elements were not found.");
        return;
    }

    const cards = Array.from(grid.querySelectorAll(".category-info-card"));

    function normalize(value) {
        return (value || "").toString().trim().toLowerCase();
    }

    function getAmount(card) {
        return Number(card.dataset.categoryAmount?.replace(",", ".") || 0);
    }

    function getPercentage(card) {
        return Number(card.dataset.categoryPercentage?.replace(",", ".") || 0);
    }

    function applySearchAndSort() {
        const query = normalize(searchInput.value);
        const sortBy = sortSelect.value;

        let filteredCards = cards.filter(card => {
            const name = normalize(card.dataset.categoryName);
            return name.includes(query);
        });

        filteredCards.sort((a, b) => {
            if (sortBy === "name") {
                return normalize(a.dataset.categoryName)
                    .localeCompare(normalize(b.dataset.categoryName));
            }

            if (sortBy === "percentage") {
                return getPercentage(b) - getPercentage(a);
            }

            return getAmount(b) - getAmount(a);
        });

        cards.forEach(card => {
            card.style.display = "none";
        });

        filteredCards.forEach(card => {
            card.style.display = "grid";
            grid.appendChild(card);
        });
    }

    searchInput.addEventListener("input", applySearchAndSort);
    sortSelect.addEventListener("change", applySearchAndSort);

    applySearchAndSort();
}