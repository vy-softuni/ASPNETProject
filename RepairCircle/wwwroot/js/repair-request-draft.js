(function () {
    const storageKey = "repairCircle.repairRequestDraft";
    const form = document.getElementById("repair-request-form");

    if (!form) {
        return;
    }

    const statusElement = document.getElementById("repair-request-draft-status");
    const restoreButton = document.getElementById("restore-repair-draft-button");
    const clearButton = document.getElementById("clear-repair-draft-button");

    const fieldNames = [
        "Input.Title",
        "Input.ItemType",
        "Input.Description",
        "Input.ImageUrl",
        "Input.LocationId",
        "Input.RepairSessionId"
    ];

    function setStatus(text) {
        if (statusElement) {
            statusElement.textContent = text || "";
        }
    }

    function readDraft() {
        try {
            const raw = localStorage.getItem(storageKey);
            return raw ? JSON.parse(raw) : null;
        } catch (error) {
            return null;
        }
    }

    function writeDraft() {
        const payload = {
            savedOn: new Date().toISOString()
        };

        fieldNames.forEach((fieldName) => {
            const field = form.querySelector(`[name="${fieldName}"]`);
            if (!field) {
                return;
            }

            payload[fieldName] = field.value;
        });

        localStorage.setItem(storageKey, JSON.stringify(payload));
        const savedOn = new Date(payload.savedOn).toLocaleString();
        setStatus(`Draft saved locally in this browser at ${savedOn}.`);
    }

    function restoreDraft(draft) {
        if (!draft) {
            setStatus("No saved draft was found in this browser.");
            return;
        }

        fieldNames.forEach((fieldName) => {
            const field = form.querySelector(`[name="${fieldName}"]`);
            if (!field || draft[fieldName] === undefined || draft[fieldName] === null) {
                return;
            }

            field.value = draft[fieldName];
        });

        const savedOn = draft.savedOn ? new Date(draft.savedOn).toLocaleString() : "an earlier time";
        setStatus(`Saved draft restored from ${savedOn}.`);
    }

    function clearDraft() {
        localStorage.removeItem(storageKey);
        setStatus("Saved draft removed from this browser.");
    }

    function formLooksEmpty() {
        return fieldNames.every((fieldName) => {
            const field = form.querySelector(`[name="${fieldName}"]`);
            return !field || (field.value || "").trim() === "";
        });
    }

    let saveTimeoutHandle = null;

    function queueSave() {
        window.clearTimeout(saveTimeoutHandle);
        saveTimeoutHandle = window.setTimeout(writeDraft, 300);
    }

    form.querySelectorAll("input, textarea, select").forEach((field) => {
        if (field.type === "file") {
            return;
        }

        field.addEventListener("input", queueSave);
        field.addEventListener("change", queueSave);
    });

    const savedDraft = readDraft();
    if (savedDraft && formLooksEmpty()) {
        restoreDraft(savedDraft);
    } else if (savedDraft) {
        setStatus("A saved local draft is available in this browser.");
    }

    if (restoreButton) {
        restoreButton.addEventListener("click", function () {
            restoreDraft(readDraft());
        });
    }

    if (clearButton) {
        clearButton.addEventListener("click", function () {
            clearDraft();
        });
    }

    form.addEventListener("submit", function () {
        localStorage.removeItem(storageKey);
    });
})();
