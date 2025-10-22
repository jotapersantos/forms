const responderAvaliacaoCompetencias = {
    avaliacaoSubmetida: false,
    inicializar() {
        this.configurarSubmitFormulario();
        this.configureMaxlenght();
        this.configureAutosize();
    },
    configurarSubmitFormulario() {
        document.querySelector('[data-responder-formulario="avaliacao-competencia"]').addEventListener("submit", (event) => {
            event.preventDefault();
            if (this.avaliacaoSubmetida === false) {
                this.enviarResposta();
            }
        });
    },
    configureMaxlenght() {
        $('.maxlength').maxlength({
            threshold: 20,
            warningClass: "badge badge-primary",
            limitReachedClass: "badge badge-success"
        });
    },
    configureAutosize() {
        document.querySelectorAll(".autosize-custom").forEach(function (textarea) {
            autosize(textarea);
        });
    },
    enviarResposta() {
        if (this.validarCamposMinLength() == false) {
            return 
        }

        const formularioForm = document.querySelector('[data-responder-formulario="avaliacao-competencia"]');

        const perguntaDoFormularioElementos = Array.from(document.querySelectorAll('[data-responder-formulario="pergunta"]'));

        const formularioFormData = new FormData();

        let buttonEnviarRespostas = formularioForm.querySelector('[data-responder-formulario="button-enviar-respostas"]');

        try {
            formularioFormData.append("Id", formularioForm.querySelector('[name="Id"]').value);
            formularioFormData.append("AvaliacaoDeCompetenciaId", formularioForm.querySelector('[name="AvaliacaoDeCompetenciaId"]').value);
            formularioFormData.append("ModeloId", formularioForm.querySelector('[name="ModeloId"]').value);
            formularioFormData.append("__RequestVerificationToken", formularioForm.querySelector('[name="__RequestVerificationToken"]').value);

            respostasFormularioManager.addRespostasEmFormData(perguntaDoFormularioElementos, formularioFormData);

            buttonEnviarRespostas.setAttribute("data-kt-indicator", "on");
            buttonEnviarRespostas.setAttribute("disabled", "true");

            this.avaliacaoSubmetida = true;

            fetch(formularioForm.getAttribute("action"), {
                method: formularioForm.getAttribute("method"),
                body: formularioFormData
            }).then(response => response.json())
                .then(response => {

                    var mensagem = response.messages.join(",");

                    if (response.isSuccess === true) {
                        toastr.success(mensagem, { timeOut: 1500 });
                        setTimeout(() => {
                            window.location.href = response.urlRedirect
                        }, 1500);
                    }
                    else {
                        let messageComplete = "";

                        for (let message of response.messages) {
                            messageComplete += message;
                        }

                        throw new Error(messageComplete);
                    }
                }).catch(error => {
                    this.avaliacaoSubmetida = false;
                    buttonEnviarRespostas.removeAttribute("data-kt-indicator");
                    buttonEnviarRespostas.removeAttribute("disabled");

                    toastr.error(error.message, { timeOut: 3000 });

                });

            
        }
        catch (error) {
            this.avaliacaoSubmetida = false;
            buttonEnviarRespostas.removeAttribute("data-kt-indicator");
            buttonEnviarRespostas.removeAttribute("disabled");

            toastr.error(error.message, { timeOut: 3000 });
        }
    },
    validarCamposMinLength() {
        const elementos = document.querySelectorAll('[minlength]')
        let camposValidos = true;

        elementos.forEach(elemento => {
            const valor = elemento.value.trim();
            const minlength = parseInt(elemento.getAttribute('minlength'), 10);

            let erroSpan = elemento.nextElementSibling;
            const mensagemErro = "O campo deve ter pelo menos " + minlength + " caracteres.";

            if (valor.length < minlength) {
                elemento.style.border = '1px solid red';

                if (!erroSpan || !erroSpan.classList.contains('erro-validacao')) {
                    erroSpan = document.createElement('span');
                    erroSpan.className = 'erro-validacao';
                    erroSpan.style.color = 'red';
                    erroSpan.style.fontSize = '12px';
                    erroSpan.textContent = mensagemErro;
                    elemento.insertAdjacentElement('afterend', erroSpan);
                }

                camposValidos = false;
            } else {
                elemento.style.border = '';

                if (erroSpan && erroSpan.classList.contains('erro-validacao')) {
                    erroSpan.remove();
                }
            }
        })

        return camposValidos;
    }
}