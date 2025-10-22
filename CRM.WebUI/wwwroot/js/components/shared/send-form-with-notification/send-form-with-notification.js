const sendFormWithNotification = {
    startSubmitForm(identifier) {
        const form = document.querySelector(identifier);
        form.addEventListener("submit", (event) => {
            event.preventDefault();
            if ($(form).valid()) {
                const formData = new FormData(form);
                fetch(form.getAttribute("action"), {
                    method: form.getAttribute("method"),
                    body: formData
                }).then(response => response.json())
                    .then(response => {
                        const message = response.messages.join("\n");
                        if (response.isSuccess === true) {
                            this.showSuccessMessage(message);
                            setTimeout(() => {
                                this.updateLocation(response.urlRedirect, response.urlReload);
                            }, 1500);
                        }
                        else {
                            throw new Error(message);
                        }
                    }).catch((errorMessage) => {
                        this.showErrorMessage(errorMessage);
                    })
            }
        })
    },
    showSuccessMessage(message) {
        toastr.success(message, { timeOut: 1500 });
    },
    showErrorMessage(message) {
        toastr.error(message, { timeOut: 3000 });
    },
    updateLocation(urlRedirect = "", urlReload = false) {
        if (urlRedirect || urlRedirect !== "") {
            window.location.href = urlRedirect;
            return;
        }
        else if (urlReload === true) {
            window.location.reload();
        }
    }
}
