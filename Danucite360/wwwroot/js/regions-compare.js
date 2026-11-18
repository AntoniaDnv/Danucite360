// Regions page: two-region comparison (from /api/dashboard/regions/{slug}) + searchable, sortable ranking table.
document.addEventListener("DOMContentLoaded", () => {
    const C = window.DanuciteCharts;
    const regions = JSON.parse(document.getElementById("regionData")?.textContent || "[]");
    const bySlug = Object.fromEntries(regions.map(r => [r.slug, r]));

    // Values are thousand EUR.
    const money = v => {
        const n = Number(v), abs = Math.abs(n);
        if (abs >= 1_000_000) return (n / 1_000_000).toLocaleString("bg-BG", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + " млрд. €";
        if (abs >= 1_000) return (n / 1_000).toLocaleString("bg-BG", { minimumFractionDigits: 1, maximumFractionDigits: 1 }) + " млн. €";
        return n.toLocaleString("bg-BG", { maximumFractionDigits: 0 }) + " хил. €";
    };
    const int = v => Number(v).toLocaleString("bg-BG", { maximumFractionDigits: 0 });
    const el = (tag, cls, text) => { const e = document.createElement(tag); if (cls) e.className = cls; if (text != null) e.textContent = text; return e; };

    // ---- Comparison ----
    const selA = document.getElementById("regionA");
    const selB = document.getElementById("regionB");
    const canvas = document.getElementById("compareChart");
    const cache = {};
    let chart = null;

    async function load(slug) {
        if (!cache[slug]) {
            const res = await fetch("/api/dashboard/regions/" + encodeURIComponent(slug));
            if (!res.ok) throw new Error("load failed");
            cache[slug] = await res.json();
        }
        return cache[slug];
    }

    function card(target, region, letter) {
        target.replaceChildren(
            el("div", "compare-letter", "Област " + letter),
            el("h3", "compare-name", region.name),
            (() => {
                const dl = el("dl", "compare-facts");
                [["Общо разходи", money(region.total)], ["На човек годишно", int(region.perCapita) + " €"], ["Население", int(region.population)]]
                    .forEach(([k, v]) => dl.append(el("dt", null, k), el("dd", "mono", v)));
                return dl;
            })(),
            Object.assign(el("a", "small", "Подробно за областта →"), { href: "/regions/" + encodeURIComponent(region.slug) })
        );
    }

    async function compare() {
        if (!selA || !selB) return;
        const a = bySlug[selA.value], b = bySlug[selB.value];
        card(document.getElementById("cardA"), a, "А");
        card(document.getElementById("cardB"), b, "Б");
        document.getElementById("thA").textContent = a.name;
        document.getElementById("thB").textContent = b.name;

        let da, db;
        try { [da, db] = await Promise.all([load(a.slug), load(b.slug)]); }
        catch { document.querySelector("#compareTable tbody").replaceChildren(el("tr", null)); return; }

        const labels = [...new Set([...da.labels, ...db.labels])];
        const valA = labels.map(l => Number(da.values[da.labels.indexOf(l)] ?? 0));
        const valB = labels.map(l => Number(db.values[db.labels.indexOf(l)] ?? 0));

        const tbody = document.querySelector("#compareTable tbody");
        tbody.replaceChildren(...labels.map((l, i) => {
            const tr = el("tr");
            const diff = valA[i] - valB[i];
            tr.append(el("td", null, l), el("td", "num", money(valA[i])), el("td", "num", money(valB[i])),
                el("td", "num " + (diff > 0 ? "diff-up" : diff < 0 ? "diff-down" : ""), (diff > 0 ? "+" : "") + money(diff)));
            return tr;
        }));

        if (!canvas || !window.Chart) return;
        const data = {
            labels,
            datasets: [
                { label: a.name, data: valA, backgroundColor: C.palette.blue, borderRadius: 3, maxBarThickness: 14 },
                { label: b.name, data: valB, backgroundColor: C.palette.navy, borderRadius: 3, maxBarThickness: 14 }
            ]
        };
        if (chart) { chart.data = data; chart.update(); return; }
        chart = new Chart(canvas, {
            type: "bar",
            data,
            options: {
                indexAxis: "y",
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: "top", align: "start" },
                    tooltip: { callbacks: { label: ctx => ctx.dataset.label + ": " + money(ctx.parsed.x) } }
                },
                scales: {
                    x: { beginAtZero: true, grid: { color: C.palette.border }, ticks: { callback: v => money(v) } },
                    y: { grid: { display: false } }
                }
            }
        });
    }

    // Comparing a region with itself says nothing: move the other picker to the next region.
    function keepDistinct(changed, other) {
        if (changed.value !== other.value) return;
        const opts = Array.from(other.options);
        other.value = opts[(opts.findIndex(o => o.value === changed.value) + 1) % opts.length].value;
    }
    selA?.addEventListener("change", () => { keepDistinct(selA, selB); compare(); });
    selB?.addEventListener("change", () => { keepDistinct(selB, selA); compare(); });
    compare();

    // ---- Ranking table ----
    const search = document.getElementById("regionSearch");
    const sortSel = document.getElementById("regionSort");
    const tbody = document.querySelector("#regionsTable tbody");
    const empty = document.getElementById("regionsEmptyState");
    if (!tbody) return;
    const rows = Array.from(tbody.querySelectorAll("tr"));

    function apply() {
        const q = (search?.value || "").trim().toLowerCase();
        const by = sortSel?.value || "percapita";
        const visible = rows.filter(r => r.dataset.name.includes(q));
        visible.sort((x, y) => by === "name"
            ? x.dataset.name.localeCompare(y.dataset.name, "bg")
            : Number(y.dataset[by]) - Number(x.dataset[by]));
        rows.forEach(r => { r.hidden = true; });
        visible.forEach(r => { r.hidden = false; tbody.appendChild(r); });
        if (empty) empty.hidden = visible.length > 0;
    }

    search?.addEventListener("input", apply);
    sortSel?.addEventListener("change", apply);
});
