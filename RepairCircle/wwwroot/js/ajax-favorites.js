(() => {
    const form = document.querySelector('[data-ajax-favorite-form]');
    if (!form) {
        return;
    }

    const button = form.querySelector('[data-favorite-toggle-button]');
    const countElement = document.getElementById('tool-favorites-count');
    const statusElement = document.getElementById('favorite-toggle-status');
    const addLabel = form.dataset.addLabel || 'Add to favorites';
    const removeLabel = form.dataset.removeLabel || 'Remove from favorites';

    const showStatus = (message, type) => {
        if (!statusElement) {
            return;
        }

        statusElement.textContent = message;
        statusElement.className = `ajax-status-message ${type === 'error' ? 'ajax-status-error' : 'ajax-status-success'}`;
        statusElement.hidden = false;
        window.clearTimeout(showStatus._timer);
        showStatus._timer = window.setTimeout(() => {
            statusElement.hidden = true;
        }, 3000);
    };

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const formData = new FormData(form);
        button.disabled = true;

        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const result = await response.json();
            if (!response.ok || !result.success) {
                showStatus(result.message || 'The favorite action could not be completed.', 'error');
                return;
            }

            if (countElement) {
                countElement.textContent = result.favoritesCount;
            }

            button.textContent = result.isFavorited ? removeLabel : addLabel;
            showStatus(result.message, 'success');
        } catch {
            showStatus('A network error interrupted the favorite update.', 'error');
        } finally {
            button.disabled = false;
        }
    });
})();
