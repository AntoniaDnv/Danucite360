// Projects Explorer: client-side filtering of project cards.
document.addEventListener("DOMContentLoaded", () => {
    const grid = document.getElementById("projectGrid");
    const search = document.getElementById("projectSearch");
    const typeSel = document.getElementById("projectType");
    const statusSel = document.getElementById("projectStatus");
    const regionSel = document.getElementById("projectRegion");
    const emptyState = document.getElementById("projectsEmptyState");

    if (!grid) return;

    const cards = Array.from(grid.querySelectorAll(".project-card"));

    function apply() {
        const q = (search?.value || "").trim().toLowerCase();
        const type = typeSel?.value || "all";
        const status = statusSel?.value || "all";
        const region = regionSel?.value || "all";

        let visible = 0;
        cards.forEach(card => {
            const match =
                (card.dataset.title || "").includes(q) &&
                (type === "all" || card.dataset.type === type) &&
                (status === "all" || card.dataset.status === status) &&
                (region === "all" || card.dataset.region === region);
            card.style.display = match ? "" : "none";
            if (match) visible++;
        });

        if (emptyState) emptyState.classList.toggle("d-none", visible > 0);
    }

    [search, typeSel, statusSel, regionSel].forEach(el => {
        if (!el) return;
        el.addEventListener("input", apply);
        el.addEventListener("change", apply);
    });

    apply();
});
