const modalSetObservador = {
    urlObterObservadores: null,
    modalInstance: null,
    btnAbrirModalObservador: null,

    inicializar(urlObterObservadores = null) {
        this.urlObterObservadores = urlObterObservadores;
    },

    abrir(urlModal, btnAbrirModalObservador) {
        this.btnAbrirModalObservador = btnAbrirModalObservador;
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

                this.configureSelectObservadores();
                //this.configureCustomValidationsInForm();
                this.configureSubmit();

                this.modalInstance = modalInstance;
            });
    },


    configureSelectObservadores() {
        if (this.urlObterObservadores) {
            const selectObservadores = document.querySelector('[name="ObservadoresId"]');

            if (!selectObservadores) {
                console.warn('Elemento select com name="ObservadoresId" não encontrado para configuração do Select2.');
                return;
            }

            $(selectObservadores).select2({
                language: "pt-BR",
                placeholder: "Selecione um observador",
                minimumInputLength: 3,
                allowClear: true,
                ajax: {
                    url: this.urlObterObservadores,
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
    /*configureCustomValidationsInForm() {
        const formularioCadastroForm = document.querySelector('[data-set-avaliador="avaliacao"]');
        $.validator.unobtrusive.parse(formularioCadastroForm);
    },*/

    configureSubmit() {
        const formElement = document.querySelector('[data-set-observador="avaliacao"]');

        if (formElement) {
            formElement.addEventListener("submit", (event) => {
                event.preventDefault();
                if (typeof $ !== 'undefined' && $(formElement).valid ? $(formElement).valid() : formElement.checkValidity()) {
                    this.salvar();
                }
            });
        }
        else {
            console.error("Formulário [data-set-observador=\"avaliacao\"] não encontrado para configurar o submit.");
        }
    },

    salvar() {
        const formElemento = document.querySelector('[data-set-observador="avaliacao"]');

        if (!formElemento) {
            console.error("Formulário [data-set-observador=\"avaliacao\"] não encontrado para salvar.");
            return;
        }

        const formData = new FormData(formElemento); // Incluirá ObservadoresId e __RequestVerificationToken se presentes no form
        const avaliacaoId = this.btnAbrirModalObservador.dataset.avaliacaoId;

        if (!avaliacaoId) {
            toastr.error("ID da avaliação não encontrado para submissão.", { timeOut: 3000 });
            return;
        }

        let actionUrl;

        let baseActionUrl = formElemento.getAttribute("action");

        if (baseActionUrl) {
            if (baseActionUrl.endsWith('/')) {
                baseActionUrl = baseActionUrl.slice(0, -1); // Remove barra final se houver
            }
            actionUrl = `${baseActionUrl}`;
        } else {
            toastr.error("URL de ação do formulário não definida.", { timeOut: 3000 });
            console.error("Atributo 'action' do formulário não encontrado ou vazio.");
            return;
        }

        fetch(actionUrl, {
            method: 'POST', // MODIFICADO: Endpoint é [HttpPost]
            body: formData
            // 'Content-Type' é definido automaticamente pelo fetch ao usar FormData
            // O token anti-falsificação deve estar no formData (via input no formulário)
        }).then(response => {
            if (!response.ok) { // Verifica se a resposta HTTP foi bem-sucedida (status 2xx)
                // Tenta ler a resposta como JSON para obter mensagens de erro do servidor, se houver
                return response.json().catch(() => {
                    // Se não for JSON, cria um erro com o status
                    throw new Error(`Erro HTTP ${response.status}: ${response.statusText}`);
                }).then(errorData => {
                    // Se conseguiu parsear JSON, usa as mensagens de erro
                    const message = errorData.messages ? errorData.messages.join("\n") : `Erro HTTP ${response.status}`;
                    throw new Error(message);
                });
            }
            return response.json(); // Converte a resposta para JSON
        }).then(responseData => { // Renomeado para responseData para clareza
            if (responseData.isSuccess === true) {
                const message = responseData.messages ? responseData.messages.join("\n") : "Operação realizada com sucesso!";
                toastr.success(message, { timeOut: 1500 });
                this.modalInstance.hide();
            } else {
                const errorMessage = responseData.messages ? responseData.messages.join("\n") : "Ocorreu um erro na operação.";
                throw new Error(errorMessage);
            }
        }).catch(error => {
            toastr.error(error.message, { timeOut: 3000 });
            console.error("Erro ao salvar observadores:", error);
        });
    },
}