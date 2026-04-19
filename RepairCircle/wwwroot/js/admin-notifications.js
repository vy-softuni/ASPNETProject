(function () {
    if (typeof signalR === "undefined") {
        return;
    }

    const listElement = document.getElementById("admin-live-notifications");
    const emptyElement = document.getElementById("admin-live-notifications-empty");
    if (!listElement) {
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/repair-circle")
        .withAutomaticReconnect()
        .build();

    connection.on("AdminNotificationReceived", function (payload) {
        if (!payload) {
            return;
        }

        if (emptyElement) {
            emptyElement.hidden = true;
        }

        const item = document.createElement("li");
        item.className = "admin-live-notification-item";
        item.innerHTML =
            `<strong>${payload.title || "Update"}</strong>` +
            `<p>${payload.message || "A new update is available."}</p>` +
            `<span>${payload.createdOnUtc || "just now"}</span>`;

        listElement.prepend(item);
    });

    connection.start().catch(function () {
        if (emptyElement) {
            emptyElement.hidden = false;
            emptyElement.textContent = "Live admin notifications are temporarily unavailable.";
        }
    });
})();
