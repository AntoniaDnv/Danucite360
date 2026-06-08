document.addEventListener("DOMContentLoaded", () => {
    const searchInput = document.getElementById("recordSearch");
    const statusFilter = document.getElementById("recordStatusFilter");
    const scopeFilter = document.getElementById("recordScopeFilter");
    const resetButton = document.getElementById("resetRecordFilters");
    const rows = Array.from(document.querySelectorAll("[data-record-row]"));
    const emptyState = document.getElementById("recordsEmptyState");

    if (!searchInput || !statusFilter || !scopeFilter || !rows.length) {
        return;
    }

    function applyFilters() {
        const search = searchInput.value.trim().toLowerCase();
        const status = statusFilter.value;
        const scope = scopeFilter.value;

        let visibleCount = 0;

        rows.forEach(row => {
            const rowSearch = row.dataset.search || "";
            const rowStatus = row.dataset.status || "";
            const rowScope = row.dataset.scope || "";

            const matchesSearch = rowSearch.includes(search);
            const matchesStatus = status === "all" || rowStatus === status;
            const matchesScope = scope === "all" || rowScope === scope;

            const shouldShow = matchesSearch && matchesStatus && matchesScope;

            row.style.display = shouldShow ? "" : "none";

            if (shouldShow) {
                visibleCount++;
            }
        });

        if (emptyState) {
            emptyState.classList.toggle("d-none", visibleCount > 0);
        }
    }

    searchInput.addEventListener("input", applyFilters);
    statusFilter.addEventListener("change", applyFilters);
    scopeFilter.addEventListener("change", applyFilters);

    if (resetButton) {
        resetButton.addEventListener("click", () => {
            searchInput.value = "";
            statusFilter.value = "all";
            scopeFilter.value = "all";
            applyFilters();
        });
    }

    applyFilters();
});