// Regional Comparison page: live filtering + sorting of the table, with the
// per-capita chart updating to reflect the current filter/sort.
document.addEventListener("DOMContentLoaded", () => {
    const C = window.DanuciteCharts;

    const search = document.getElementById("regionSearch");
    const statusSel = document.getElementById("regionStatus");
    const sortSel = document.getElementById("regionSort");
    const tbody = document.querySelector("#regionsTable tbody");
    const emptyState = document.getElementById("regionsEmptyState");
    const canvas = document.getElementById("perCapitaChart");

    if (!tbody) return;

    const rows = Array.from(tbody.querySelectorAll("tr"));

    const data = rows.map(row => ({
        row,
        name: row.dataset.regionName || "",
        status: row.dataset.status || "demo",
        percapita: Number(row.dataset.percapita || 0),
        spending: Number(row.dataset.spending || 0),
        population: Number(row.dataset.population || 0)
    }));

    let chart = null;
    if (canvas && window.Chart) {
        chart = new Chart(canvas, {
            type: "bar",
            data: {
                labels: [],
                datasets: [{
                    data: [],
                    backgroundColor: C.palette.teal,
                    borderRadius: 4,
                    maxBarThickness: 56
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: { callbacks: { label: ctx => "€" + ctx.parsed.y.toLocaleString() + " / resident" } }
                },
                scales: {
                    x: { grid: { display: false } },
                    y: { beginAtZero: true, grid: { color: C.palette.border }, ticks: { callback: v => "€" + v.toLocaleString() } }
                }
            }
        });
    }

    function apply() {
        const q = (search?.value || "").trim().toLowerCase();
        const status = statusSel?.value || "all";
        const sortBy = sortSel?.value || "percapita";

        let visible = data.filter(d =>
            d.name.includes(q) && (status === "all" || d.status === status));

        visible.sort((a, b) => {
            if (sortBy === "name") return a.name.localeCompare(b.name);
            if (sortBy === "spending") return b.spending - a.spending;
            if (sortBy === "population") return b.population - a.population;
            return b.percapita - a.percapita;
        });

        // Reorder + toggle table rows.
        data.forEach(d => { d.row.style.display = "none"; });
        visible.forEach(d => { d.row.style.display = ""; tbody.appendChild(d.row); });

        if (emptyState) emptyState.classList.toggle("d-none", visible.length > 0);

        // Update chart: top 5 of the current view, by the chosen metric.
        if (chart) {
            const metric = sortBy === "spending" ? "spending"
                : sortBy === "population" ? "population" : "percapita";
            const top = [...visible].sort((a, b) => b[metric] - a[metric]).slice(0, 5);
            chart.data.labels = top.map(d => d.row.querySelector("a")?.textContent.trim() || d.name);
            chart.data.datasets[0].data = top.map(d => d[metric]);
            const isMoney = metric !== "population";
            chart.options.scales.y.ticks.callback = v => isMoney ? "€" + v.toLocaleString() : v.toLocaleString();
            chart.update();
        }
    }

    search?.addEventListener("input", apply);
    statusSel?.addEventListener("change", apply);
    sortSel?.addEventListener("change", apply);

    apply();
});
