const modalSetAvaliador = {
    urlObterAvaliadores: null,
    modalInstance: null,
    btnAvaliador: null,
    inicializar(urlObterAvaliadores = null) {
        this.urlObterAvaliadores = urlObterAvaliadores;
    },
    abrir(urlModal, btnAvaliador) {
        this.btnAvaliador = btnAvaliador;
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

                this.configureSelectAvaliadores();
                this.configureCustomValidationsInForm();
                this.configureSubmit();

                this.modalInstance = modalInstance;
            });
    },


    configureSelectAvaliadores() {
        if (this.urlObterAvaliadores) {
            const selectAvaliadores = document.querySelector('[name="AvaliadorId"]');

            $(selectAvaliadores).select2({
                language: "pt-BR",
                placeholder: "Selecione um avaliador",
                minimumInputLength: 3,
                allowClear: true,
                ajax: {
                    url: this.urlObterAvaliadores,
                    dataType: 'json',
                    delay: 250,
                    data: (params) => {
                        return {
                            searchString: params.term,
                            quantity: 10,
                        };
                    },
                    processResults: (data) => {
                        var formattedData = $.map(data, function (value, key) {
                            return {
                                id: key,
                                text: value
                            };
                        });

                        return {
                            results: formattedData
                        };
                    },
                    cache: true
                }
            });
        }
    },
    configureCustomValidationsInForm() {
        const formularioCadastroForm = document.querySelector('[data-set-avaliador="avaliacao"]');
        $.validator.unobtrusive.parse(formularioCadastroForm);
    },
    configureSubmit() {
        document.querySelector('[data-set-avaliador="avaliacao"]').addEventListener("submit", (event) => {
            event.preventDefault();
            if ($('[data-set-avaliador="avaliacao"]').valid()) {
                this.salvar();
            }
        });
    },
    salvar() {
        const formElemento = document.querySelector('[data-set-avaliador="avaliacao"]');

        const formData = new FormData(formElemento);

        fetch(formElemento.getAttribute("action"), {
            method: formElemento.getAttribute("method"),
            body: formData
        }).then(response => response.json())
            .then(response => {
                if (response.isSuccess === true) {
                    const message = response.messages.join("\n");
                    toastr.success(message, { timeOut: 1500 });
                    if (response.data) {
                        this.alterarAvaliadorAtual(response.data.id, response.data.name)
                    }
                }
                else {
                    const error = new Error(response.messages);
                    throw error;
                }
            }).catch(error => {
                toastr.error(error.message, { timeOut: 3000 });
            });
    },

    alterarAvaliadorAtual(idNovoAvaliador, nomeNovoAvaliador) {
        const avaliadorDiv = `div_${ this.btnAvaliador.dataset.avaliacaoId}_${ this.btnAvaliador.dataset.avaliadorId }`
        const divAvaliador = document.getElementById(avaliadorDiv)
        divAvaliador.innerHTML = nomeNovoAvaliador;
        divAvaliador.id = `div_${this.btnAvaliador.dataset.avaliacaoId}_${idNovoAvaliador}`
        this.btnAvaliador.dataset.avaliadorId = idNovoAvaliador;
        this.modalInstance.hide();
    }
}