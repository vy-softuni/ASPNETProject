(() => {
    const root = document.getElementById('feedback-ajax-root');
    if (!root) {
        return;
    }

    const listUrl = root.dataset.listUrl;
    const listHost = root.querySelector('[data-feedback-list-host]');
    const formHost = root.querySelector('[data-feedback-form-host]');
    const statusBox = root.querySelector('[data-feedback-status]');
    const openButton = document.querySelector('[data-open-feedback-form]');
    const refreshButton = root.querySelector('[data-reload-feedback-list]');

    const setStatus = (message, isError = false) => {
        if (!statusBox) {
            return;
        }

        statusBox.textContent = message;
        statusBox.className = `ajax-status-message ${isError ? 'ajax-status-error' : 'ajax-status-success'}`;
        statusBox.hidden = false;
        window.clearTimeout(setStatus._timer);
        setStatus._timer = window.setTimeout(() => {
            statusBox.hidden = true;
        }, 3500);
    };

    const loadFeedbackList = async () => {
        if (!listUrl || !listHost) {
            return;
        }

        listHost.innerHTML = '<div class="loading-panel">Loading feedback…</div>';

        try {
            const response = await fetch(listUrl, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            listHost.innerHTML = await response.text();
        } catch {
            listHost.innerHTML = '<div class="empty-state compact-empty"><h3>Feedback could not be loaded</h3><p>Please refresh the page and try again.</p></div>';
        }
    };

    const openForm = async () => {
        if (!openButton || !formHost) {
            return;
        }

        const formUrl = openButton.dataset.formUrl;
        if (!formUrl) {
            return;
        }

        formHost.innerHTML = '<div class="loading-panel">Loading feedback form…</div>';

        try {
            const response = await fetch(formUrl, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                formHost.innerHTML = '<div class="empty-state compact-empty"><h3>Feedback form unavailable</h3><p>You may have already left feedback for this request.</p></div>';
                return;
            }

            formHost.innerHTML = await response.text();
            wireFormEvents();
        } catch {
            formHost.innerHTML = '<div class="empty-state compact-empty"><h3>Feedback form unavailable</h3><p>Please try again in a moment.</p></div>';
        }
    };

    const closeForm = () => {
        if (formHost) {
            formHost.innerHTML = '';
        }
    };

    const wireFormEvents = () => {
        const form = formHost?.querySelector('[data-ajax-feedback-form]');
        const closeButton = formHost?.querySelector('[data-close-feedback-form]');
        if (closeButton) {
            closeButton.addEventListener('click', closeForm);
        }

        if (!form) {
            return;
        }

        form.addEventListener('submit', async (event) => {
            event.preventDefault();
            const formData = new FormData(form);
            const submitButton = form.querySelector('button[type="submit"]');
            if (submitButton) {
                submitButton.disabled = true;
            }

            try {
                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                const contentType = response.headers.get('content-type') || '';
                if (contentType.includes('application/json')) {
                    const result = await response.json();
                    if (!response.ok || !result.success) {
                        setStatus(result.message || 'Feedback could not be saved.', true);
                        return;
                    }

                    closeForm();
                    setStatus(result.message || 'Feedback saved successfully.');
                    if (openButton) {
                        openButton.remove();
                    }
                    await loadFeedbackList();
                    return;
                }

                formHost.innerHTML = await response.text();
                wireFormEvents();
            } catch {
                setStatus('A network error interrupted the feedback request.', true);
            } finally {
                if (submitButton) {
                    submitButton.disabled = false;
                }
            }
        });
    };

    if (openButton) {
        openButton.addEventListener('click', openForm);
    }

    if (refreshButton) {
        refreshButton.addEventListener('click', loadFeedbackList);
    }

    loadFeedbackList();
})();
