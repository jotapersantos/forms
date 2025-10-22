const modalAddAvaliadosAvaliacao = {
    abrir(urlModal) {
        fetch(urlModal, {
            headers: {
                'Content-Type': 'text/html'
            }
        }).then(response => response.text())
            .then(responseText => {
                const modalElemento = document.querySelector('[data-modal="modal"]');

                modalElemento.querySelector(".modal-content").innerHTML = responseText;

                const modalInstance = new bootstrap.Modal(modalElemento);

                modalInstance.show();

                this.configureSelectTwo();

                this.configureSubmit();
            });
    },
    configureSelectTwo() {
        $('[data-control="select2"]').select2();
    },
    configureSubmit() {
        document.querySelector('[data-add-gabarito-avaliacao="formulario"]').addEventListener("submit", (event) => {
            event.preventDefault();
            this.salvar();
        })
    },
    salvar() {
        const addAvaliadosEmAvaliacaoForm = document.querySelector('[data-add-gabarito-avaliacao="formulario"]');

        const avaliacaoId = addAvaliadosEmAvaliacaoForm.querySelector('[name*="AvaliacaoId"]').value;

        const avaliadosIds = Array.from(addAvaliadosEmAvaliacaoForm.querySelector('[name*="AvaliadosIds"]').selectedOptions).map(option => option.value);

        //const addAvaliadosEmAvaliacaoJson = {
        //    avaliacaoId: avaliacaoId,
        //    avaliadosIds: avaliacoesIds
        //}

        fetch(addAvaliadosEmAvaliacaoForm.getAttribute("action"), {
            body: JSON.stringify({
                avaliacaoId: avaliacaoId,
                avaliadosIds: avaliadosIds,
            }),
            method: addAvaliadosEmAvaliacaoForm.getAttribute("method"),
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": addAvaliadosEmAvaliacaoForm.querySelector('[name="__RequestVerificationToken"]').value
            }
        }).then(response => response.json())
            .then(response => {
                const message = response.messages.join("\n");

                if (response.isSuccess) {
                    toastr.success(message, { timeOut: 1500 });

                    setTimeout(() => {
                        window.location.href = response.urlRedirect;
                    }, 1500);
                }
                else {
                    throw new Error(message);
                }
            }).catch(error => {
                toastr.error(error.message, { timeOut: 3000 });
            })
        
    }
}