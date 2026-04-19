(function () {
    const storageKey = "repairCircle.toolFilters";
    const form = document.getElementById("tool-filter-form");
    if (!form) {
        return;
    }

    const message = document.getElementById("saved-tool-filters-message");
    const saveButton = document.getElementById("save-tool-filters-button");
    const clearButton = document.getElementById("clear-tool-filters-button");

    function getPayload() {
        const formData = new FormData(form);
        return {
            searchTerm: (formData.get("SearchTerm") || "").toString(),
            categoryId: (formData.get("CategoryId") || "").toString(),
            locationId: (formData.get("LocationId") || "").toString(),
            condition: (formData.get("Condition") || "").toString(),
            onlyAvailable: form.querySelector('[name="OnlyAvailable"]')?.checked === true,
            savedOn: new Date().toISOString()
        };
    }

    function setMessage(text) {
        if (message) {
            message.textContent = text || "";
        }
    }

    function readSaved() {
        try {
            const raw = localStorage.getItem(storageKey);
            return raw ? JSON.parse(raw) : null;
        } catch (error) {
            return null;
        }
    }

    function writeSaved(payload) {
        localStorage.setItem(storageKey, JSON.stringify(payload));
    }

    function clearSaved() {
        localStorage.removeItem(storageKey);
    }

    function formHasActiveValues() {
        return Array.from(form.elements).some((element) => {
            if (!element.name) {
                return false;
            }
            if (element.type === "checkbox") {
                return element.checked;
            }
            return (element.value || "").toString().trim() !== "";
        });
    }

    function restoreSavedFilters(payload) {
        if (!payload) {
            return;
        }

        const mappings = {
            SearchTerm: payload.searchTerm,
            CategoryId: payload.categoryId,
            LocationId: payload.locationId,
            Condition: payload.condition
        };

        Object.keys(mappings).forEach((key) => {
            const field = form.querySelector(`[name="${key}"]`);
            if (field) {
                field.value = mappings[key] || "";
            }
        });

        const onlyAvailableField = form.querySelector('[name="OnlyAvailable"]');
        if (onlyAvailableField) {
            onlyAvailableField.checked = payload.onlyAvailable === true;
        }
    }

    const saved = readSaved();
    const hasQueryString = new URLSearchParams(window.location.search).toString().length > 0;

    if (!hasQueryString && !formHasActiveValues() && saved) {
        restoreSavedFilters(saved);
        setMessage("Saved filters restored from this browser.");
    }

    form.addEventListener("submit", function () {
        writeSaved(getPayload());
    });

    if (saveButton) {
        saveButton.addEventListener("click", function () {
            writeSaved(getPayload());
            setMessage("Current filters saved locally in this browser.");
        });
    }

    if (clearButton) {
        clearButton.addEventListener("click", function () {
            clearSaved();
            setMessage("Saved filters removed from this browser.");
        });
    }
})();
