// Home: "Къде отиват" as a ranked list with thin bars (a share reads better as a list than a donut).
document.addEventListener("DOMContentLoaded", async () => {
    const list = document.getElementById("spendingList");
    if (!list) return;

    const res = await fetch("/api/dashboard/categories");
    if (!res.ok) {
        list.textContent = "Данните не можаха да се заредят.";
        return;
    }

    const data = await res.json();
    const total = data.values.reduce((a, b) => a + Number(b), 0);
    const rows = data.labels
        .map((label, i) => ({ label, value: Number(data.values[i]) }))
        .sort((a, b) => b.value - a.value);
    const max = rows.length ? rows[0].value : 1;

    rows.forEach((r, i) => {
        const row = document.createElement("div");
        row.className = "revenue-bar-row";

        const label = document.createElement("span");
        label.className = "revenue-bar-label";
        label.textContent = r.label;

        const track = document.createElement("div");
        track.className = "revenue-bar-track";
        const fill = document.createElement("span");
        if (i > 0) fill.className = "alt"; // only the largest share carries the accent
        fill.style.width = (r.value / max * 100).toFixed(1) + "%";
        track.append(fill);

        const value = document.createElement("span");
        value.className = "revenue-bar-value mono";
        value.textContent = total ? (r.value / total * 100).toLocaleString("bg-BG", { maximumFractionDigits: 1 }) + "%" : "—";

        row.append(label, track, value);
        list.append(row);
    });
});
