(function () {
    const projects = JSON.parse(document.getElementById('projectData').textContent);
    const bySlug = Object.fromEntries(projects.map(p => [p.slug, p]));
    const panelEmpty = document.getElementById('panelEmpty');
    const panelBody = document.getElementById('panelBody');
    const regionSelect = document.getElementById('mapRegion');
    const markers = {};
    let map = null;

    const statusColor = { 'Completed': '#16A34A', 'Ongoing': '#2563EB', 'In Planning': '#64748B' };

    function el(tag, cls, text) {
        const e = document.createElement(tag);
        if (cls) e.className = cls;
        if (text != null) e.textContent = text;
        return e;
    }

    function progress(label, pct, cls) {
        const wrap = el('div', 'mb-3');
        const head = el('div', 'd-flex justify-content-between small mb-1');
        head.append(el('span', null, label), el('strong', 'num', pct == null ? 'няма данни' : pct + '%'));
        const bar = el('div', 'progress');
        bar.setAttribute('role', 'progressbar');
        bar.setAttribute('aria-label', label);
        bar.setAttribute('aria-valuenow', pct ?? 0);
        bar.setAttribute('aria-valuemin', 0);
        bar.setAttribute('aria-valuemax', 100);
        const fill = el('div', 'progress-bar ' + cls);
        fill.style.width = (pct ?? 0) + '%';
        bar.append(fill);
        wrap.append(head, bar);
        return wrap;
    }

    function fact(dl, term, value) {
        dl.append(el('dt', null, term), el('dd', null, value));
    }

    function show(slug) {
        const p = bySlug[slug];
        if (!p) return;
        panelBody.replaceChildren();

        const badges = el('div', 'd-flex flex-wrap gap-2 mb-2');
        badges.append(el('span', 'status-badge status-draft', p.status), el('span', 'chip-muted', p.type));
        if (p.isDemo) badges.append(el('span', 'status-badge status-demo', 'Демо'));

        const dl = el('dl', 'measure-facts mb-3');
        fact(dl, 'Място', p.city ? p.city + ' (приблизително)' : p.regionName);
        fact(dl, 'Общ бюджет', p.budget);
        fact(dl, 'Изплатено досега', p.spent ?? 'няма данни');
        fact(dl, 'Основен източник', p.funder);
        fact(dl, 'Източник на данните', p.isDemo ? 'Демонстрационен запис — не е официален' : 'Официален източник');

        const link = el('a', 'btn btn-primary btn-sm', 'Пълна информация →');
        link.href = '/projects/' + encodeURIComponent(p.slug);

        panelBody.append(
            badges,
            el('h2', 'measure-title', p.title),
            el('p', 'small text-muted', p.description),
            progress('Физически напредък', p.physicalPercent, 'bg-primary'),
            progress('Финансов напредък (изплатено)', p.financialPercent, 'bg-success'),
            dl,
            link);
        panelEmpty.hidden = true;
        panelBody.hidden = false;

        if (map && markers[slug]) {
            map.setView(markers[slug].getLatLng(), Math.max(map.getZoom(), 9));
            markers[slug].openPopup();
        }
    }

    function filterRegion(region) {
        document.querySelectorAll('.projects-table tbody tr').forEach(tr => {
            tr.hidden = region !== 'all' && tr.dataset.region !== region;
        });
        if (!map) return;
        const visible = [];
        projects.forEach(p => {
            const m = markers[p.slug];
            if (!m) return;
            const on = region === 'all' || p.regionSlug === region;
            if (on) { m.addTo(map); visible.push(m.getLatLng()); } else { m.remove(); }
        });
        if (visible.length) map.fitBounds(L.latLngBounds(visible).pad(0.4), { maxZoom: 11 });
    }

    function initMap() {
        if (typeof L === 'undefined') throw new Error('Leaflet not loaded');
        map = L.map('projectMap', { scrollWheelZoom: false }).setView([42.75, 25.3], 7);
        // shortcut: public OSM tiles are fine for demo traffic; use a keyed tile provider for production load.
        const tiles = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
            maxZoom: 18
        }).addTo(map);
        let tileErrors = 0;
        tiles.on('tileerror', () => { if (++tileErrors === 6) document.getElementById('mapFallback').hidden = false; });

        projects.filter(p => p.lat != null).forEach(p => {
            const m = L.circleMarker([p.lat, p.lng], {
                radius: 9, weight: 2, color: '#fff',
                fillColor: statusColor[p.statusKey] || '#2563EB', fillOpacity: 0.9
            }).addTo(map);
            m.bindPopup(() => el('strong', null, p.title));
            m.bindTooltip(p.title);
            m.on('click', () => show(p.slug));
            markers[p.slug] = m;
        });
    }

    try {
        initMap();
    } catch (e) {
        document.getElementById('projectMap').hidden = true;
        document.getElementById('mapFallback').hidden = false;
    }

    document.querySelectorAll('.project-pick').forEach(b => b.addEventListener('click', () => show(b.dataset.slug)));
    regionSelect.addEventListener('change', () => filterRegion(regionSelect.value));
})();
