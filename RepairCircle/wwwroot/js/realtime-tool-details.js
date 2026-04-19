(function () {
    const config = window.repairCircleToolRealtime;
    if (!config || typeof signalR === "undefined") {
        return;
    }

    const quantityElement = document.getElementById("tool-quantity");
    const badgeElement = document.getElementById("tool-availability-badge");
    const statusElement = document.getElementById("tool-live-status");

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/repair-circle")
        .withAutomaticReconnect()
        .build();

    connection.on("ToolAvailabilityChanged", function (payload) {
        if (!payload || payload.toolId !== config.toolId) {
            return;
        }

        if (quantityElement) {
            quantityElement.textContent = payload.quantity;
        }

        if (badgeElement) {
            badgeElement.textContent = payload.isAvailable ? "Available" : "Unavailable";
            badgeElement.classList.remove("status-success", "status-muted");
            badgeElement.classList.add(payload.isAvailable ? "status-success" : "status-muted");
        }

        if (statusElement) {
            statusElement.textContent = payload.message || "Tool availability changed.";
        }
    });

    connection.start()
        .then(function () {
            return connection.invoke("SubscribeToTool", config.toolId);
        })
        .then(function () {
            if (statusElement) {
                statusElement.textContent = config.initiallyAvailable
                    ? "Live updates connected. The tool is currently available."
                    : "Live updates connected. The tool is currently unavailable.";
            }
        })
        .catch(function () {
            if (statusElement) {
                statusElement.textContent = "Live updates are temporarily unavailable.";
            }
        });

    window.addEventListener("beforeunload", function () {
        if (connection.state === signalR.HubConnectionState.Connected) {
            connection.invoke("UnsubscribeFromTool", config.toolId).catch(function () { });
        }
    });
})();
