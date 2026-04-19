(function () {
    const storageKey = "repairCircle.recentTools";
    const maxItems = 6;

    function readItems() {
        try {
            const raw = localStorage.getItem(storageKey);
            return raw ? JSON.parse(raw) : [];
        } catch (error) {
            return [];
        }
    }

    function writeItems(items) {
        localStorage.setItem(storageKey, JSON.stringify(items));
    }

    function upsertCurrentTool() {
        if (!window.repairCircleRecentTool) {
            return;
        }

        const current = {
            id: window.repairCircleRecentTool.id,
            name: window.repairCircleRecentTool.name,
            url: window.repairCircleRecentTool.url,
            categoryName: window.repairCircleRecentTool.categoryName,
            locationName: window.repairCircleRecentTool.locationName,
            imageUrl: window.repairCircleRecentTool.imageUrl,
            isAvailable: window.repairCircleRecentTool.isAvailable,
            viewedOn: new Date().toISOString()
        };

        const existing = readItems().filter((item) => item.id !== current.id);
        existing.unshift(current);
        writeItems(existing.slice(0, maxItems));
    }

    function buildCard(item) {
        const article = document.createElement("article");
        article.className = "recent-tool-item";

        const link = document.createElement("a");
        link.href = item.url;
        link.className = "recent-tool-link";

        if (item.imageUrl) {
            const image = document.createElement("img");
            image.src = item.imageUrl;
            image.alt = item.name;
            image.className = "recent-tool-image";
            link.appendChild(image);
        }

        const content = document.createElement("div");
        content.className = "recent-tool-content";

        const title = document.createElement("strong");
        title.textContent = item.name;
        content.appendChild(title);

        const meta = document.createElement("span");
        meta.className = "muted small-text";
        meta.textContent = `${item.categoryName} • ${item.locationName}`;
        content.appendChild(meta);

        const status = document.createElement("span");
        status.className = `status-badge ${item.isAvailable ? "status-success" : "status-muted"}`;
        status.textContent = item.isAvailable ? "Available" : "Unavailable";
        content.appendChild(status);

        link.appendChild(content);
        article.appendChild(link);

        return article;
    }

    function renderLists() {
        const items = readItems();
        document.querySelectorAll("[data-recent-tools-root]").forEach((root) => {
            const list = root.querySelector("[data-recent-tools-list]");
            if (!list) {
                return;
            }

            list.innerHTML = "";

            if (!items.length) {
                root.hidden = true;
                return;
            }

            root.hidden = false;
            items.forEach((item) => list.appendChild(buildCard(item)));
        });
    }

    upsertCurrentTool();
    renderLists();
})();
