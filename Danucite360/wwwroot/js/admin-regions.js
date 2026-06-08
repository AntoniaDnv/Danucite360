document.addEventListener("DOMContentLoaded", () => {
    const searchInput = document.getElementById("regionAdminSearch");
    const statusFilter = document.getElementById("regionStatusFilter");
    const sortSelect = document.getElementById("regionSort");
    const resetButton = document.getElementById("resetRegionFilters");
    const tableBody = document.getElementById("regionsTableBody");
    const rows = Array.from(document.querySelectorAll("[data-region-row]"));
    const emptyState = document.getElementById("regionsEmptyState");

    if (!searchInput || !statusFilter || !sortSelect || !tableBody || !rows.length) {
        return;
    }

    function normalize(value) {
        return (value || "").toString().trim().toLowerCase();
    }

    function applyFilters() {
        const query = normalize(searchInput.value);
        const status = statusFilter.value;
        const sortBy = sortSelect.value;

        let filteredRows = rows.filter(row => {
            const search = normalize(row.dataset.search);
            const rowStatus = row.dataset.status || "";

            const matchesSearch = search.includes(query);
            const matchesStatus = status === "all" || rowStatus === status;

            return matchesSearch && matchesStatus;
        });

        filteredRows.sort((a, b) => {
            if (sortBy === "population") {
                return Number(b.dataset.population) - Number(a.dataset.population);
            }

            return normalize(a.dataset.name).localeCompare(normalize(b.dataset.name));
        });

        rows.forEach(row => {
            row.style.display = "none";
        });

        filteredRows.forEach(row => {
            row.style.display = "";
            tableBody.appendChild(row);
        });

        if (emptyState) {
            emptyState.classList.toggle("d-none", filteredRows.length > 0);
        }
    }

    searchInput.addEventListener("input", applyFilters);
    statusFilter.addEventListener("change", applyFilters);
    sortSelect.addEventListener("change", applyFilters);

    if (resetButton) {
        resetButton.addEventListener("click", () => {
            searchInput.value = "";
            statusFilter.value = "all";
            sortSelect.value = "name";
            applyFilters();
        });
    }

    applyFilters();
});