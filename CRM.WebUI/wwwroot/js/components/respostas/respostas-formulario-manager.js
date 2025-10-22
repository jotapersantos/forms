const respostasFormularioManager = {
    addRespostasEmFormData(perguntasComRespostaElementos, formularioFormData) {
        const respostasCriadas = perguntasComRespostaElementos.map((perguntaElemento) => {
            return this.converterRespostaDaPerguntaEmJson(perguntaElemento);
        });

        if (this.todasPerguntasObrigatoriasRespondidas(respostasCriadas) === true) {
            respostasCriadas.forEach((resposta) => {
                if (this.perguntaPossuiRespostas(resposta) === true) {
                    formularioFormData.append(`Respostas.Index`, resposta.perguntaId);
                    formularioFormData.append(`Respostas[${resposta.perguntaId}].PerguntaId`, resposta.perguntaId);
                    formularioFormData.append(`Respostas[${resposta.perguntaId}].Obrigatorio`, resposta.obrigatorio);
                    formularioFormData.append(`Respostas[${resposta.perguntaId}].Tipo`, resposta.tipo);

                    this.addRespostasEmFormDataPorTipo(resposta, formularioFormData);
                }
            });
        }
        else {
            this.validarPerguntasDoFormularioPorRespostas(respostasCriadas);
            throw new Error("É necessário responder as perguntas obrigatórias.");
        }        
    },
    converterRespostaDaPerguntaEmJson(perguntaElemento) {
        const perguntaId = perguntaElemento.dataset.perguntaId;

        const respostaEmJson = {
            perguntaId: perguntaElemento.querySelector(`[name="Respostas[${perguntaId}].PerguntaId"]`).value,
            obrigatorio: perguntaElemento.querySelector(`[name="Respostas[${perguntaId}].Obrigatorio"]`).value === "true",
            tipo: perguntaElemento.querySelector(`[name="Respostas[${perguntaId}].Tipo"]`).value,
            texto: perguntaElemento.querySelector(`[name="Respostas[${perguntaId}].Texto"]`)?.value,
        }

        respostaEmJson["alternativasSelecionadasIds"] = this.obterAlternativasSelecionadasIdsDoTipoObjetivo(perguntaElemento, respostaEmJson.perguntaId, respostaEmJson.tipo);

        return respostaEmJson;
    },

    obterAlternativasSelecionadasIdsDoTipoObjetivo(perguntaElemento, perguntaId, tipo) {
        if (tipo === "MultiplaEscolha" || tipo === "CaixaSelecao") {
            return Array.from(perguntaElemento
                .querySelectorAll(`[name="Respostas[${perguntaId}].AlternativasSelecionadasIds"]:checked`))
                .map(alternativa => alternativa.value);
        }

        return this.obterAlternativaSelecionadaDoTipoListaSuspensa(perguntaElemento, perguntaId);
    },
    obterAlternativaSelecionadaDoTipoListaSuspensa(perguntaElemento, perguntaId) {
        const alternativaValue = perguntaElemento.querySelector(`[name="Respostas[${perguntaId}].AlternativasSelecionadasIds"]`)?.value;

        if (!alternativaValue) {
            return [];
        }

        return [alternativaValue];
    },

    isTipoDiscursivo(pergunta) {
        return pergunta.tipo === "TextoCurto" || pergunta.tipo === "TextoLongo";
    },
    isTipoObjetivo(pergunta) {
        return pergunta.tipo === "MultiplaEscolha" || pergunta.tipo === "CaixaSelecao" || pergunta.tipo === "ListaSuspensa";
    },
    exibirErroDePerguntaObrigatoria(pergunta) {
        let perguntaElemento = document.querySelector(`[data-responder-formulario="pergunta"][data-pergunta-id="${pergunta.perguntaId}"]`);

        perguntaElemento.classList.add("border-danger");

        perguntaElemento.querySelector('[data-responder-formulario="pergunta-mensagem-obrigatoria"]').classList.remove("d-none");
    },
    esconderErroDePerguntaObrigatoria(pergunta) {
        let perguntaElemento = document.querySelector(`[data-responder-formulario="pergunta"][data-pergunta-id="${pergunta.perguntaId}"]`);

        perguntaElemento.classList.remove("border-danger");

        perguntaElemento.querySelector('[data-responder-formulario="pergunta-mensagem-obrigatoria"]').classList.add("d-none");
    },
    validarPerguntaDoFormulario(perguntaId) {
        let perguntaElemento = document.querySelector(`[data-responder-formulario="pergunta"][data-pergunta-id="${perguntaId}"]`);

        let respostaDaPerguntaJson = this.converterRespostaDaPerguntaEmJson(perguntaElemento);

        if (respostaDaPerguntaJson.obrigatorio === true && this.perguntaPossuiRespostas(respostaDaPerguntaJson) === false) {
            this.exibirErroDePerguntaObrigatoria(respostaDaPerguntaJson);
        }
        else if (respostaDaPerguntaJson.obrigatorio === true && this.perguntaPossuiRespostas(respostaDaPerguntaJson) === true) {
            this.esconderErroDePerguntaObrigatoria(respostaDaPerguntaJson);
        }
    },
    validarPerguntasDoFormularioPorRespostas(respostasJson) {
        respostasJson.forEach((resposta) => {
            this.validarPerguntaDoFormulario(resposta.perguntaId);
        });
    },
    todasPerguntasObrigatoriasRespondidas(perguntas) {
        let totalDePerguntasObrigatorias = perguntas.filter(pergunta => pergunta.obrigatorio === true).length;

        let totalDePerguntasObrigatoriasRespondidas = perguntas.filter(pergunta => this.perguntaPossuiRespostas(pergunta) === true && pergunta.obrigatorio === true).length;

        return totalDePerguntasObrigatorias === totalDePerguntasObrigatoriasRespondidas;
    },
    perguntaPossuiRespostas(pergunta) {
        if (pergunta.tipo === "TextoCurto" || pergunta.tipo === "TextoLongo") {
            return pergunta.texto != null && pergunta.texto != undefined && pergunta.texto.length > 0;
        }
        else if (pergunta.tipo === "MultiplaEscolha" || pergunta.tipo === "ListaSuspensa" || pergunta.tipo === "CaixaSelecao") {
            return pergunta.alternativasSelecionadasIds.length >= 1;
        }
        else {
            throw new Error("Ocorreu um erro ao enviar o formulário. Não foi possível verificar se as perguntas obrigatórias foram respondidas");
        }
    },
    addRespostasEmFormDataPorTipo(respostaDaPergunta, formularioFormData) {
        if (this.isTipoDiscursivo(respostaDaPergunta)) {
            formularioFormData.append(`Respostas[${respostaDaPergunta.perguntaId}].Texto`, respostaDaPergunta.texto);
        }
        else if (this.isTipoObjetivo(respostaDaPergunta)) {
            respostaDaPergunta.alternativasSelecionadasIds.forEach((alternativa) => {
                formularioFormData.append(`Respostas[${respostaDaPergunta.perguntaId}].AlternativasSelecionadasIds`, alternativa);
            });
        }
        else {
            throw new Error("Ocorreu um erro ao enviar o formulário. Não foi possível verificar se as perguntas obrigatórias foram respondidas");
        }
    },
}