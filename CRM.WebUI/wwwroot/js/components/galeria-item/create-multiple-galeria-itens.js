const createMultipleGaleriaItens = {
    uploadFilesChunk: null,
    urlListagemGaleriaItem: null,
    processoEnvioItensIniciado: null,
    tamanhoMaximoLotes: 10,
    modalProgressoEnvioGaleriaItens: null,
    extensoesPermitidas: [],
    salvamentoIniciado: false,
    inicializar(uploadFilesChunk, urlListagemGaleriaItem, extensoesPermitidas) {
        this.uploadFilesChunk = uploadFilesChunk;
        this.urlListagemGaleriaItem = urlListagemGaleriaItem;
        this.extensoesPermitidas = extensoesPermitidas;
        this.configurarSubmit();
    },
    configurarSubmit() {
        document.querySelector('[data-create-multiple-galeria-itens="formulario"]').addEventListener("submit", async (event) => {
            event.preventDefault();
            if (this.salvamentoIniciado === false) {
                this.salvamentoIniciado = true;
                this.exibirModalProgressoEnvioGaleriaItens();
                this.toggleButtonSalvar();
                await this.criarItensDeGaleria();
            }
        });
    },
    async criarItensDeGaleria() {
        try {
            const createMultipleGaleriaItensForm = document.querySelector('[data-create-multiple-galeria-itens="formulario"]');
            const midiaUploadsFiles = Array.from(createMultipleGaleriaItensForm.querySelector('[name="MidiaUploadList"]').files);

            if (this.arquivosAdicionadosPossuemExtensaoInvalida(midiaUploadsFiles) == true) {
                throw new Error(`Alguns itens possuem extensões inválidas para o tipo de galeria. Extensões permitidas: ${this.extensoesPermitidas.join(",")}`);
            }

            const tipoItemGaleria = createMultipleGaleriaItensForm.querySelector('[name="TipoItem"]').value;
            const galeriaId = createMultipleGaleriaItensForm.querySelector('[name="GaleriaId"]').value;
            const urlAction = createMultipleGaleriaItensForm.getAttribute("action");
            const tokenRequisicao = createMultipleGaleriaItensForm.querySelector('[name="__RequestVerificationToken"]').value;

            const totalLotes = Math.ceil(midiaUploadsFiles.length / this.tamanhoMaximoLotes);

            let contadorLotes = 0;

            const requisicaoPromises = [];

            for (let indice = 0; indice < midiaUploadsFiles.length; indice += this.tamanhoMaximoLotes) {
                const midiaUploadsParaLote = midiaUploadsFiles.slice(indice, indice + this.tamanhoMaximoLotes);

                const galeriaItens = [];

                for (const midia of midiaUploadsParaLote) {
                    const galeriaItem = await this.criarGaleriaItemJson(midia, tipoItemGaleria, galeriaId);
                    galeriaItens.push(galeriaItem);
                }

                contadorLotes++;

                requisicaoPromises.push(this.enviarLoteGaleriaItens(galeriaItens, contadorLotes, totalLotes, urlAction, tokenRequisicao));
            }

            const resultados = await Promise.allSettled(requisicaoPromises);

            const hasErrors = resultados.some(resultado => resultado.isSuccess = false);

            if (hasErrors) {
                toastr.error("Alguns itens não foram enviados. Tente enviar novamente.", { timeOut: 1500 });
            }
            else {
                toastr.success("Os itens de galeria foram criados com sucesso.", { timeOut: 1500 });
            }

            setTimeout(() => {
                window.location.href = this.urlListagemGaleriaItem;
            }, 1500);
        }
        catch (error) {
            this.pararSalvamento();
            toastr.error(`Não foi possível criar os itens de galeria: ${error.message}`, { timeOut: 1500 });
        }
    },
    async criarGaleriaItemJson(fileUpload, tipoItemGaleria, galeriaId) {
        try {
            const midiaLocation = await uploadFilesChunk.initUploadMultipartComRetornoDoLocation(fileUpload);
            const itemNome = fileUpload.name.substring(0, fileUpload.name.lastIndexOf('.'));

            return {
                nome: itemNome,
                midia: midiaLocation,
                tipoItem: Number(tipoItemGaleria),
                galeriaId: galeriaId
            };
        }
        catch (error) {
            this.pararSalvamento();
            toastr.error(`Não foi possível criar os itens de galeria: ${error.message}`, { timeOut: 1500 });
        }
    },
    async enviarLoteGaleriaItens(galeriaItensLote, loteAtual, totalLotes, url, tokenRequest) {
        try {
            const response = await fetch(url, {
                method: "post",
                headers: {
                    "Content-Type": 'application/json',
                    "RequestVerificationToken": tokenRequest
                },
                body: JSON.stringify({
                    itens: galeriaItensLote,
                    numeroLote: loteAtual,
                    totalLotes: totalLotes,
                })
            });

            this.atualizarProgressoEnvioGaleriaItens(loteAtual, totalLotes);

            return await response.json();
        }
        catch (error) {
            this.pararSalvamento();
            toastr.error(`Não foi possível criar os itens de galeria: ${error.message}`, { timeOut: 1500 });
        }
    },
    exibirModalProgressoEnvioGaleriaItens() {
        this.modalProgressoEnvioGaleriaItens = new bootstrap.Modal(document.querySelector('[data-create-multiple-galeria-itens="modal-progresso"]'), {
            backdrop: 'static',
            keyboard: false
        });

        this.modalProgressoEnvioGaleriaItens.show();
    },
    esconderModalProgressoEnvioGaleriaItens() {
        if (this.modalProgressoEnvioGaleriaItens) {
            this.modalProgressoEnvioGaleriaItens.hide();
            this.modalProgressoEnvioGaleriaItens = null;
        }
    },
    atualizarProgressoEnvioGaleriaItens(loteAtual, totalLotes) {
        const barraProgresso = document.querySelector('[data-create-multiple-galeria-itens="barra-progresso"]');

        const percentualLote = (loteAtual / totalLotes) * 100;
        const percentualLoteText = `${percentualLote.toFixed(2)}%`;

        barraProgresso.style.width = percentualLoteText;
        barraProgresso.innerText = percentualLoteText;
    },
    arquivosAdicionadosPossuemExtensaoInvalida(arquivos) {
        return arquivos.some(arquivo => {
            const extensaoArquivo = arquivo.name.substring(arquivo.name.lastIndexOf('.')).toLowerCase();
            return this.extensoesPermitidas.includes(extensaoArquivo) == false;
        });
    },
    toggleButtonSalvar() {
        const buttonSalvarFormulario = document.querySelector('[data-create-multiple-galeria-itens="button-salvar"]');
        if (this.salvamentoIniciado === true) {
            buttonSalvarFormulario.setAttribute("data-kt-indicator", "on");
            buttonSalvarFormulario.disabled = true;
        }
        else {
            buttonSalvarFormulario.disabled = false;
            buttonSalvarFormulario.removeAttribute("data-kt-indicator");
        }
    },
    pararSalvamento() {
        this.salvamentoIniciado = false;
        this.esconderModalProgressoEnvioGaleriaItens();
        this.toggleButtonSalvar();
    }
}