const responderFormulario = {
    formularioSubmetido: false,
    inicializar() {
        this.configurarSubmitFormulario();
    },
    configurarSubmitFormulario() {
        document.querySelector('[data-responder-formulario="formulario"]').addEventListener("submit", (event) => {
            event.preventDefault();

            if (this.formularioSubmetido === false) {
                this.enviarResposta();
            }
        });
    },
    enviarResposta() {
        const formularioForm = document.querySelector('[data-responder-formulario="formulario"]');

        const perguntaDoFormularioElementos = Array.from(document.querySelectorAll('[data-responder-formulario="pergunta"]'));

        const formularioFormData = new FormData();

        let buttonEnviarRespostas = formularioForm.querySelector('[data-responder-formulario="button-enviar-respostas"]');

        try
        {
            formularioFormData.append("FormularioId", formularioForm.querySelector('[name="FormularioId"]').value);
            formularioFormData.append("ModeloId", formularioForm.querySelector('[name="ModeloId"]').value);
            formularioFormData.append("__RequestVerificationToken", formularioForm.querySelector('[name="__RequestVerificationToken"]').value);

            //Add respostas do formulario ao formData
            respostasFormularioManager.addRespostasEmFormData(perguntaDoFormularioElementos, formularioFormData);

            buttonEnviarRespostas.setAttribute("data-kt-indicator", "on");
            buttonEnviarRespostas.setAttribute("disabled", "true");

            this.formularioSubmetido = true;

            fetch(formularioForm.getAttribute("action"), {
                method: formularioForm.getAttribute("method"),
                body: formularioFormData
            }).then(response => response.json())
                .then(response => {
                    buttonEnviarRespostas.removeAttribute("data-kt-indicator");

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
                    throw error;
                });
        }
        catch (error) {
            this.formularioSubmetido = false;

            buttonEnviarRespostas.removeAttribute("disabled");

            toastr.error(error.message, { timeOut: 3000 });
        }
    },
}