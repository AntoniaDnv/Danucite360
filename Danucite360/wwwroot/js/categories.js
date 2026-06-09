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
    const C = window.DanuciteCharts;
    const total = data.values.reduce((a, b) => a + Number(b), 0);

    new Chart(canvas, {
        type: "bar",
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: `Spending Categories ${data.year}`,
                    data: data.values,
                    backgroundColor: C ? C.palette.teal : "#0D9488",
                    borderRadius: 4,
                    maxBarThickness: 26
                }
            ]
        },
        options: {
            indexAxis: "y",
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const pct = total ? ((context.parsed.x / total) * 100).toFixed(1) : 0;
                            return `${context.parsed.x.toLocaleString()} ${data.unit} (${pct}%)`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: { color: C ? C.palette.border : "#E2E8F0" },
                    ticks: { callback: v => Number(v).toLocaleString() }
                },
                y: { grid: { display: false } }
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