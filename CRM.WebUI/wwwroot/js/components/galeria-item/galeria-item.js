const galeriaItem = {
	uploadFilesChunk: null,
	formularioEnviado: false,
	galeriaTipos: {
		Imagem: 1,
		Video: 2,
		Arquivo: 3,
		PerguntasFrequentes: 4
	},
	inicializar(uploadFilesChunk) {
		this.uploadFilesChunk = uploadFilesChunk;
		this.uploadFilesChunk.adicionarCallbackFinalizar(galeriaItem.salvar);
		this.configurarMaxLength();
		this.configurarEnvioFormulario();
	},
	configurarEnvioFormulario() {
		document.querySelector('[data-galeria-item="formulario"]').addEventListener("submit", async (event) => {
			event.preventDefault();

			if ($('[data-galeria-item="formulario"]').valid() && this.formularioEnviado === false) {
				this.formularioEnviado = true;
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
		const galeriaItemDocumentoFormulario = document.querySelector('[data-galeria-item="formulario"]');
		const galeriaTipo = Number(galeriaItemDocumentoFormulario.querySelector('[name="TipoGaleria"]').value);

		if (galeriaTipo === this.galeriaTipos.PerguntasFrequentes) {
			this.salvar();
			return;
		}

		this.toggleModoLeituraEmCamposDoFormulario();

		const midiaUploadFiles = galeriaItemDocumentoFormulario.querySelector('[name="MidiaUpload"]').files[0];

		if (midiaUploadFiles) {
			await this.uploadFilesChunk.initUploadMultipart(midiaUploadFiles, () => {
				this.formularioEnviado = false;
				this.toggleModoLeituraEmCamposDoFormulario(galeriaItemDocumentoFormulario);
				this.toggleButtonSalvar();
			});
		}
		else {
			this.salvar();
		}
	},
	salvar(midiaUrl = null) {
		const galeriaItemDocumentoFormulario = document.querySelector('[data-galeria-item="formulario"]');
		const formData = new FormData(galeriaItemDocumentoFormulario);
		const galeriaTipo = Number(formData.get("TipoGaleria"));

		if (midiaUrl) {
			formData.delete("MidiaUpload");
			formData.append("Midia", midiaUrl);
		}

		fetch(galeriaItemDocumentoFormulario.getAttribute('action'), {
			method: galeriaItemDocumentoFormulario.getAttribute('method'),
			body: formData
		}).then(res => res.json())
			.then( (data) => {
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

					if (galeriaTipo !== this.galeriaTipos.PerguntasFrequentes) {
						this.toggleModoLeituraEmCamposDoFormulario();
					}
					this.toggleBotaoSalvar();
					toastr.error(messages, { timeOut: 3000 });
				}
			});
	},
	toggleModoLeituraEmCamposDoFormulario(galeriaItemFormulario = null) {
		if (!galeriaItemFormulario) {
			galeriaItemFormulario = document.querySelector('[data-galeria-item="formulario"]');
		}

		if (this.formularioEnviado === true) {
			galeriaItemFormulario.querySelector('[name="Nome"]').setAttribute("readonly", true);
			galeriaItemFormulario.querySelector('[name="TipoItem"]').setAttribute("readonly", true);
			galeriaItemFormulario.querySelector('[name="Descricao"]').setAttribute("readonly", true);
			galeriaItemFormulario.querySelector('[name="TipoItem"]').setAttribute("readonly", true);
			galeriaItemFormulario.querySelector('[name="MidiaUpload"]').setAttribute("disabled", '');
		}
		else {
			galeriaItemFormulario.querySelector('[name="Nome"]').removeAttribute("readonly");
			galeriaItemFormulario.querySelector('[name="TipoItem"]').removeAttribute("readonly");
			galeriaItemFormulario.querySelector('[name="Descricao"]').removeAttribute("readonly");
			galeriaItemFormulario.querySelector('[name="TipoItem"]').removeAttribute("readonly");
			galeriaItemFormulario.querySelector('[name="MidiaUpload"]').removeAttribute("disabled");
		}
	},
	toggleButtonSalvar() {
		const buttonSalvarFormulario = document.querySelector('[data-galeria-item="button-salvar"]');
		if (this.formularioEnviado === true)
		{
			buttonSalvarFormulario.setAttribute("data-kt-indicator", "on");
			buttonSalvarFormulario.disabled = true;
		}
		else
		{
			buttonSalvarFormulario.disabled = false;
			buttonSalvarFormulario.removeAttribute("data-kt-indicator");
		}
	}
}