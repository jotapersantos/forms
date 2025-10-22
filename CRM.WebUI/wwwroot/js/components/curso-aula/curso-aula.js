const cursoAula = {
    uploadFileChunk: null,
    formularioEnviado: false,
    campoAulaTextoEditorInstance: null,
    inicializar(uploadFileChunk) {
        this.uploadFileChunk = uploadFileChunk;
        this.uploadFileChunk.adicionarCallbackFinalizar(cursoAula.salvar);
        this.configurarMaxLength();
        this.configurarEnvioFormulario();
    },
    configurarEnvioFormulario() {
        document.querySelector('[data-curso-aula="formulario"]').addEventListener("submit", async (event) => {
            event.preventDefault();
            if ($('[data-curso-aula="formulario"]').valid()) {
                this.formularioEnviado = true;
                this.toggleModoLeituraEmCamposDoFormulario();
                this.toggleButtonSalvar();
                await this.iniciarEnvioFormulario();
            }
        })
    },
    configurarMaxLength() {
        $('.maxlength').maxlength({
            threshold: 20,
            warningClass: "badge badge-primary",
            limitReachedClass: "badge badge-success"
        });
    },
    async iniciarEnvioFormulario() {
        const cursoAulaFormulario = document.querySelector('[data-curso-aula="formulario"]');
        const aulaVideoFiles = cursoAulaFormulario.querySelector('[name="AulaVideoUpload"]').files[0];

        if (aulaVideoFiles) {
            await this.uploadFileChunk.initUploadMultipart(aulaVideoFiles, () => {
                this.formularioEnviado = false;
                this.toggleModoLeituraEmCamposDoFormulario(cursoAulaFormulario);
                this.toggleButtonSalvar();
            }, "modulos/cursos/aulas");
        }
        else {
            this.salvar();
        }
    },
    salvar(aulaVideoUrl = null) {
        const cursoAulaFormulario = document.querySelector('[data-curso-aula="formulario"]');
        const formData = new FormData(cursoAulaFormulario);

        if (aulaVideoUrl) {
            formData.delete("AulaVideoUpload");
            formData.append("AulaVideoUrl", aulaVideoUrl);
        }

        fetch(cursoAulaFormulario.getAttribute('action'), {
            method: cursoAulaFormulario.getAttribute('method'),
            body: formData
        }).then(res => res.json())
            .then((data) => {
                let messages = "";
                data.messages.forEach(function (message) {
                    messages += `${message}\n`;
                });
                if (data.isSuccess === true) {
                    toastr.success(messages, { timeOut: 1500 });
                    setTimeout(function () {
                        window.location.href = data.urlRedirect;
                    }, 1500);
                }
                else {
                    this.formularioEnviado = false;
                    this.toggleModoLeituraEmCamposDoFormulario();
                    this.toggleBotaoSalvar();
                    toastr.error(messages, { timeOut: 3000 });
                }
            });
    },
    toggleModoLeituraEmCamposDoFormulario(cursoAulaFormulario = null) {
        if (cursoAulaFormulario) {
            cursoAulaFormulario = document.querySelector('[data-curso-aula="formulario"]');
        }

        if (this.formularioEnviado === true) {
            document.querySelector("#Nome").setAttribute("readonly", true);
            document.querySelector("#Duracao").setAttribute("readonly", true);
            document.querySelector("#AulaTexto").setAttribute("readonly", true);
            document.querySelector("#AulaVideoUpload").setAttribute("disabled", '');
        }
        else {
            document.querySelector("#Nome").removeAttribute("readonly");
            document.querySelector("#Duracao").removeAttribute("readonly");
            document.querySelector("#AulaTexto").removeAttribute("readonly");
            document.querySelector("#AulaVideoUpload").removeAttribute("disabled");
        }
    },
    toggleButtonSalvar() {
        const buttonSalvarFormulario = document.querySelector('[data-curso-aula="button-salvar"]');
        if (this.formularioEnviado === true) {
            buttonSalvarFormulario.setAttribute("data-kt-indicator", "on");
            buttonSalvarFormulario.disabled = true;
        }
        else {
            buttonSalvarFormulario.disabled = false;
            buttonSalvarFormulario.removeAttribute("data-kt-indicator");
        }
    },
    toggleCampoAtivo(campoAtivo) {
        campoAtivo.value = campoAtivo.checked;
    }
}
