document.addEventListener("DOMContentLoaded", () => {
    const searchInput = document.getElementById("regionSearch");
    const regionTiles = document.querySelectorAll(".region-tile");

    if (!searchInput || !regionTiles.length) {
        return;
    }

    searchInput.addEventListener("input", () => {
        const query = searchInput.value.trim().toLowerCase();

        regionTiles.forEach(tile => {
            const name = tile.dataset.regionName || "";
            const isVisible = name.includes(query);

            tile.style.display = isVisible ? "flex" : "none";
        });
    });
});