

const formularioModeloCadastro = {
    urlAddSecao: null,
    urlAddPergunta: null,
    urlAlternativa: null,
    draggableListaPerguntas: [],
    draggableListaAlternativas: [],
    modoOrdenacao: false,
    /**INICIO - Inicialização da biblioteca **/
    inicializar(urlAddSecao, urlAddPergunta, urlAlternativa) {
        this.urlAddSecao = urlAddSecao;
        this.urlAddPergunta = urlAddPergunta;
        this.urlAlternativa = urlAlternativa;
        this.configureComponents();
    },
    /**FIM - Inicialização da biblioteca **/

    /**INICIO - Configuração dos componentes da tela de cadastro de modelo de formulário **/
    configureComponents() {
        this.configureSecoesEmModelo();

        this.configurePlugins();

        this.configureValidacoesEmCampos();

        this.configureEnvio();
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
    configureValidacoesEmCampos() {
        const formularioModeloCadastroForm = $('[data-formulario-modelo="formulario"]');

        $(formularioModeloCadastroForm).removeData("validator");

        $(formularioModeloCadastroForm).removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse(formularioModeloCadastroForm);
    },
    configurePlugins() {
        this.configureMaxlenght();

        this.configureAutosize();
    },
    configureEnvio() {
        document.querySelector('[data-formulario-modelo="formulario"]').addEventListener("submit", event => {
            event.preventDefault();

            if ($('[data-formulario-modelo="formulario"]').valid()) {
                this.salvar();
            }
        });
    },
    /**FIM - Configuração dos componentes da tela de cadastro de modelo de formulário **/
    /** INICIO - Gerenciamento dos temas adicionados (Somente Edição) **/
    removerImagemTema() {
        const inputTema = document.querySelector('[data-formulario-modelo="input-tema"]');
        const temaView = document.querySelector('[data-formulario-modelo="tema-view"]');

        inputTema.classList.remove('d-none');
        temaView.classList.add('d-none');

        document.querySelector('[name="TemaRemovido"]').value = true;
    },
    /** FIM - Gerenciamento dos temas adicionados **/

    /** INICIO - Gerenciamento das seções do modelo **/
    addSecao() {
        fetch(this.urlAddSecao, {
            method: 'get',
            headers: {
                "Content-Type": 'text/html'
            }
        }).then(response => response.text())
            .then(response => this._renderizarSecao(response));
    },
    _renderizarSecao(response) {
        const secoesContainer = document.querySelector('[data-formulario-modelo="secao-container"]');

        const secaoElemento = this.converterTextoEmHTML(response).body.firstChild;

        secoesContainer.append(secaoElemento);

        this.configurePlugins();

        this.configurePerguntasDaSecao(secaoElemento);

        this.toggleTituloEmSecoesDoModelo();

        this.configureValidacoesEmCampos();

        this.selecionarCampoTitulodaSecaoAtual(secaoElemento);

        this.atualizarOrdemDasSecoes();
    },
    selecionarCampoTitulodaSecaoAtual(secaoElemento) {
        const campoTituloDaSecao = secaoElemento.querySelector('[name*="Titulo"]');

        const offsetSecaoElemento = secaoElemento.getBoundingClientRect().top - 100;

        window.scrollBy({ top: offsetSecaoElemento, behavior: 'smooth' });

        setTimeout(() => {
            campoTituloDaSecao.focus()
        }, 500);
    },
    removerSecao(secaoId) {
        if (this.obterTotalSecoesDoModelo() == 1) {
            return;
        }

        document.querySelector(`[data-formulario-modelo="secao"][data-id="${secaoId}"]`).remove();

        this.toggleTituloEmSecoesDoModelo();

        this.atualizarOrdemDasSecoes();
    },
    atualizarOrdemDasSecoes() {
        const secoesDoModelo = document.querySelectorAll(`[data-formulario-modelo="secao"]`);

        if (secoesDoModelo.length > 0) {
            secoesDoModelo.forEach(function (secao, index) {
                secao.querySelector("[name*='Ordem']").value = index + 1;
            });
        }
    },
    configureSecoesEmModelo() {
        const secoesEmModelo = document.querySelectorAll("[data-formulario-modelo='secao']");

        const totalSecoes = secoesEmModelo.length;

        this.toggleTituloEmSecoesDoModelo(secoesEmModelo);

        secoesEmModelo.forEach((secao, index) => {
            this.configurePerguntasDaSecao(secao);
        });
    },
    toggleTituloEmSecoesDoModelo(secoesEmModelo = []) {
        if (secoesEmModelo.length === 0) {
            secoesEmModelo = document.querySelectorAll("[data-formulario-modelo='secao']");
        }

        const totalSecoes = secoesEmModelo.length;

        secoesEmModelo.forEach((secao, index) => {
            if (totalSecoes > 1) {
                this.exibirButtonRemoverSecao(secao);

                this.exibirTituloDaSecao(secao, index + 1, totalSecoes);
            }
            else {
                this.esconderButtonRemoverSecao(secao);

                this.esconderTituloDaSecao(secao);
            }
        });
    },
    exibirTituloDaSecao(secao, numeroSecao, totalSecoes) {
        const secaoCabecalho = secao.querySelector("[data-formulario-modelo='secao-cabecalho']");

        secaoCabecalho.querySelector('[data-formulario-modelo="secao-titulo"]').innerText = `Seção ${numeroSecao} de ${totalSecoes}`;

        secaoCabecalho.classList.remove("d-none");
    },
    esconderTituloDaSecao(secao) {
        const secaoCabecalho = secao.querySelector("[data-formulario-modelo='secao-cabecalho']");

        secaoCabecalho.classList.add("d-none");
    },
    exibirButtonRemoverSecao(secao) {
        secao.querySelector('[data-formulario-modelo="button-remover-secao"]').classList.remove('d-none');
    },
    esconderButtonRemoverSecao(secao) {
        secao.querySelector('[data-formulario-modelo="button-remover-secao"]').classList.add('d-none');
    },
    obterTotalSecoesDoModelo() {
        return document.querySelectorAll('[data-formulario-modelo="secao"]').length;
    },

    formularioPossuiSecoesEPerguntasEOpcoes(formulario) {
        const secoesDoModeloElementos = Array.from(formulario.querySelectorAll('[data-formulario-modelo="secao"]'));

        let resultado = true;

        if (secoesDoModeloElementos.length === 0) {
            return false;
        }

        for (let secaoElemento of secoesDoModeloElementos) {

            resultado = this.secoesPossuemPerguntasEAlternativas(secaoElemento);

            if (resultado == false) {
                break;
            }
        }

        return resultado;
    },
    secoesPossuemPerguntasEAlternativas(secaoElemento) {
        const perguntasDaSecao = Array.from(secaoElemento.querySelectorAll('[data-formulario-modelo="pergunta"]'));
        let resultado = true;

        if (perguntasDaSecao.length === 0) {
            return false;
        }

        for (let perguntaElemento of perguntasDaSecao) {
            let perguntaTipo = perguntaElemento.querySelector('[name*="Tipo"]').value;

            if (this.isPerguntaObjetiva(perguntaTipo) === true) {
                resultado = this.perguntaObjetivaPossuiAlternativas(perguntaElemento);
            }

            if (resultado === false) {
                break;
            }
        }

        return resultado;
    },

    /** FIM - Gerenciamento das seções do modelo **/

    /** INICIO - Ordernar Perguntas da Seção **/
    abrirModalParaOrdenarPerguntas(secaoId) {
        const listaPerguntas = Array.from(document.querySelectorAll(`[data-formulario-modelo="pergunta"][data-secao-id="${secaoId}"]`))
            .map((pergunta) => {
                const perguntaTitulo = pergunta.querySelector('[name*="Enunciado"]').value ? pergunta.querySelector('[name*="Enunciado"]').value : "";

                return {
                    id: pergunta.dataset.id,
                    titulo: perguntaTitulo,
                    tipo: pergunta.querySelector('[name*="Tipo"]')?.value
                }
            });

        modalOrdernarPerguntasModelo.abrir(listaPerguntas, secaoId, this.atualizarOrdemDasPerguntas.bind(this));
    },
    atualizarOrdemDasPerguntas(secaoId, listaPerguntaIds) {
        const perguntaContainer = document.querySelector(`[data-formulario-modelo="secao"][data-id="${secaoId}"]`)
            .querySelector('[data-formulario-modelo="pergunta-container"]');

        listaPerguntaIds.forEach((perguntaId) => {
            const perguntaElemento = perguntaContainer.querySelector(`[data-formulario-modelo="pergunta"][data-id="${perguntaId}"]`);
            perguntaContainer.appendChild(perguntaElemento);
        });

        this.atualizarOrdemDasPerguntasEmSecao(secaoId);
    },
    /** FIM - Ordernar Perguntas da Seção **/

    /** INICIO - Gerenciamento das perguntas do modelo **/
    async addPergunta(secaoId, tipo, perguntaDuplicadaJson = null) {
        try {
            const response = await this.getPerguntaHTML(secaoId, tipo);

            const contentType = response.headers.get('content-type');

            if (contentType && contentType.includes('application/json')) {
                const responseJson = await response.json();
                throw new Error(responseJson.messages.join("\n"));
            }

            const perguntaHTML = await response.text();

            const perguntaElemento = this.converterTextoEmHTML(perguntaHTML).body.firstChild;

            this._renderizarPergunta(perguntaElemento);

            if (perguntaDuplicadaJson) {
                this.preencherPerguntaComConteudoDuplicado(perguntaDuplicadaJson, perguntaElemento)
            }
        }

        catch (error) {
            toastr.error(error.message, { timeOut: 3000 });
        }
    },
    async preencherPerguntaComConteudoDuplicado(perguntaDuplicadaJson, perguntaElemento) {
        perguntaElemento.querySelector('[name*="Enunciado"]').value = perguntaDuplicadaJson.enunciado;
        perguntaElemento.querySelector('[name*="Descricao"]').value = perguntaDuplicadaJson.descricao;
        perguntaElemento.querySelector('[name*="Obrigatorio"]').checked = perguntaDuplicadaJson.obrigatorio;

        this.toggleCheckboxValue(perguntaElemento.querySelector('[name*="Obrigatorio"]'));

        if (this.isPerguntaDiscursiva(perguntaDuplicadaJson.tipo)) {
            this.preencherCamposIntervaloCaracteresEmPerguntaElemento(perguntaDuplicadaJson, perguntaElemento);
        }

        else if (this.isPerguntaObjetiva(perguntaDuplicadaJson.tipo)) {
            this.addAlternativasPreenchidasEmPerguntaElemento(perguntaDuplicadaJson, perguntaElemento);
        }

        if (this.isPerguntaTextoCurto(perguntaDuplicadaJson.tipo)) {
            this.preencherCampoFormatoTextoEmPerguntaElementoTextoCurto(perguntaDuplicadaJson, perguntaElemento);
        }
    },
    async getPerguntaHTML(secaoId, tipo) {
        return await fetch(`${this.urlAddPergunta}?secaoId=${secaoId}&tipo=${tipo}`, {
            method: 'get',
            headers: {
                "Content-Type": 'application/json'
            }
        });
    },
    _renderizarPergunta(perguntaElemento) {
        const perguntasContainer = document.querySelector(`[data-formulario-modelo="secao"][data-id="${this.obterIdDaSecaoDaPergunta(perguntaElemento)}"]`)
            .querySelector('[data-formulario-modelo="pergunta-container"]');

        this.configureOrdemDaPerguntaEmSecao(perguntaElemento);

        const tipoDaPergunta = perguntaElemento.querySelector('[name*="Tipo"]').value;

        if (this.isPerguntaObjetiva(tipoDaPergunta)) {
            this.configurarOrdenacaoEmAlternativasDaPergunta(perguntaElemento);
        }

        perguntasContainer.appendChild(perguntaElemento);

        this.configurePlugins();

        this.configureValidacoesEmCampos();

        this.selecionarCampoEnunciadoDaPergunta(perguntaElemento);
    },
    selecionarCampoEnunciadoDaPergunta(perguntaElemento) {
        const campoEnunciadoDaPergunta = perguntaElemento.querySelector('[name*="Enunciado"]');

        const offsetPerguntaElemento = perguntaElemento.getBoundingClientRect().top - 100;

        window.scrollBy({ top: offsetPerguntaElemento, behavior: 'smooth' });

        setTimeout(() => {
            campoEnunciadoDaPergunta.focus()
        }, 500);
    },
    removerPergunta(perguntaId) {
        const perguntaElemento = document.querySelector(`[data-formulario-modelo="pergunta"][data-id="${perguntaId}"]`);

        const secaoId = perguntaElemento.dataset.secaoId;

        perguntaElemento.remove();

        this.atualizarOrdemDasPerguntasEmSecao(secaoId);
    },
    configurePerguntasDaSecao(secaoElemento) {
        const perguntasDaSecao = secaoElemento.querySelectorAll('[data-formulario-modelo="pergunta"]');

        perguntasDaSecao.forEach((perguntaElemento) => {
            if (this.isPerguntaObjetiva(perguntaElemento.querySelector('[name*="Tipo"]').value)) {
                this.configurarOrdenacaoEmAlternativasDaPergunta(perguntaElemento);
            }
        });
    },
    validateQuantidadeMinimaCaracteres(campo) {

        const campoQuantidadeMaximaName = campo.dataset.campoQuantidadeMaxima;
        const campoQuantidadeMaxima = document.querySelector(`[name="${campoQuantidadeMaximaName}"]`);
        const quantidadeMaxima = Number(campoQuantidadeMaxima.value);

        const quantidadeMinima = Number(campo.value);
        const minValue = Number(campo.min);
        const maxValue = Number(campo.max);

        if (this.isQuantidadeCaracteresEmIntervaloInvalido(quantidadeMinima, minValue, maxValue) === true) {
            this.exibirMensagemErroValidacao(campo.name, `O valor de Quantidade Mínima de Caracteres deve estar entre ${minValue} e ${maxValue}`);
        }
        else if (this.isQuantidadeMinimaCaracteresMaiorQueQuantidadeMaxima(quantidadeMinima, quantidadeMaxima) === true) {
            this.exibirMensagemErroValidacao(campo.name, `O valor de Quantidade Mínima de Caracteres não pode ser maior que Quantidade Máxima de Caracteres`);
        }
        else {
            this.esconderMensagemErroValidacao(campo.name);
        }
    },

    validateQuantidadeMaximaCaracteres(campo) {
        const quantidadeMaxima = Number(campo.value);
        const minValue = Number(campo.min);
        const maxValue = Number(campo.max);

        if (this.isQuantidadeCaracteresEmIntervaloInvalido(quantidadeMaxima, minValue, maxValue) === true) {
            this.exibirMensagemErroValidacao(campo.name, `O valor de Quantidade Maxima de Caracteres deve estar entre ${minValue} e ${maxValue}`);
        }
        else {
            this.esconderMensagemErroValidacao(campo.name);
        }

        const campoQuantidadeMinimaName = campo.dataset.campoQuantidadeMinima;
        const campoQuantidadeMinima = document.querySelector(`[name="${campoQuantidadeMinimaName}"]`);

        this.validateQuantidadeMinimaCaracteres(campoQuantidadeMinima);
    },
    isQuantidadeCaracteresEmIntervaloInvalido(quantidade, minValue, maxValue) {
        if (quantidade) {
            return quantidade < minValue || quantidade > maxValue;
        }
        return false;
    },
    isQuantidadeMinimaCaracteresMaiorQueQuantidadeMaxima(quantidadeMinima, quantidadeMaxima) {
        return quantidadeMinima > quantidadeMaxima;
    },
    exibirMensagemErroValidacao(name, mensagem) {
        const mensagemErroElemento = document.querySelector(`[data-valmsg-for="${name}"]`);
        mensagemErroElemento.innerText = mensagem;
        mensagemErroElemento.style.display = "";
    },
    esconderMensagemErroValidacao(name) {
        const mensagemErroElemento = document.querySelector(`[data-valmsg-for="${name}"]`);
        mensagemErroElemento.innerText = "";
        mensagemErroElemento.style.display = "none";
    },
    configureOrdemDaPerguntaEmSecao(perguntaElemento) {
        const secaoDaPerguntaElemento = this.obterIdDaSecaoDaPergunta(perguntaElemento);

        perguntaElemento.querySelector('[name*="Ordem"]').value = this.obterTotalDePerguntasEmSecao(secaoDaPerguntaElemento) + 1;
    },
    atualizarOrdemDasPerguntasEmSecao(secaoId) {
        const perguntasDaSecao = document.querySelector(`[data-formulario-modelo='secao'][data-id="${secaoId}"]`)
            .querySelectorAll('[data-formulario-modelo="pergunta"]');

        perguntasDaSecao.forEach(function (pergunta, index) {
            pergunta.querySelector("[name*='Ordem']").value = index + 1;
        });
    },
    obterIdDaSecaoDaPergunta(perguntaElemento) {
        return perguntaElemento.dataset.secaoId;
    },
    obterTotalDePerguntasEmSecao(secaoId) {
        return document.querySelector(`[data-formulario-modelo="secao"][data-id="${secaoId}"]`)
            .querySelectorAll('[data-formulario-modelo="pergunta"]')
            .length;
    },
    isPerguntaObjetiva(tipo) {
        return tipo === 'MultiplaEscolha' || tipo === 'CaixaSelecao' || tipo === 'ListaSuspensa';
    },
    isPerguntaDiscursiva(tipo) {
        return tipo === "TextoCurto" || tipo === "TextoLongo"
    },
    isPerguntaTextoCurto(tipo) {
        return tipo === "TextoCurto";
    },
    toggleModoLeituraEmOrdenacaoDePerguntas(btn, secaoId) {
        if (this.ordenacaoSecaoItemAtivaEmSecao(secaoId) === true) {
            this.toggleOrdenacaoAtivaEmSecaoDoFormulario(secaoId, false);

            this.toggleBotaoAcoesSecao(secaoId, true);

            this.exibirAreaEditavelEmSecaoItensEmSecao(secaoId);

            btn.innerText = "Ordenar Perguntas";

            this.ativarBotaoSalvarFormulario();
        }
        else {
            this.toggleOrdenacaoAtivaEmSecaoDoFormulario(secaoId, true);

            this.toggleBotaoAcoesSecao(secaoId, false);

            this.esconderAreaEditavelEmSecaoItensEmSecao(secaoId);

            btn.innerText = "Desativar Ordenação";

            this.desativarBotaoSalvarFormulario();
        }
    },
    preencherCamposIntervaloCaracteresEmPerguntaElemento(perguntaJson, perguntaElemento) {
        perguntaElemento.querySelector('[name*="QuantidadeMinimaCaracteres"]').value = perguntaJson.quantidadeMinimaCaracteres;
        perguntaElemento.querySelector('[name*="QuantidadeMaximaCaracteres"]').value = perguntaJson.quantidadeMaximaCaracteres;
    },
    preencherCampoFormatoTextoEmPerguntaElementoTextoCurto(perguntaJson, perguntaElemento) {
        perguntaElemento.querySelector('[name*="FormatoTexto"]').value = perguntaJson.formatoTexto;
    },
    addAlternativasPreenchidasEmPerguntaElemento(perguntaJson, perguntaElemento) {
        const primeiraAlternativa = perguntaJson.alternativas[0];

        const primeiraAlternativaElemento = perguntaElemento.querySelector('[data-formulario-modelo="alternativa"]');

        this.preencherAlternativaElementoComValoresDuplicados(primeiraAlternativa, primeiraAlternativaElemento);

        if (perguntaJson.alternativas.length > 1) {
            const alternativasRestantes = perguntaJson.alternativas.slice(1);

            alternativasRestantes.forEach(async (alternativa, index) => {
                const perguntaId = perguntaElemento.dataset.id;
                const secaoId = perguntaJson.secaoId;
                const tipo = perguntaJson.tipo;

                await this.addAlternativa(perguntaId, secaoId, tipo, alternativa);
            });
        }
    },
    perguntaObjetivaPossuiAlternativas(perguntaElemento) {
        const alternativasDaPerguntaObjetiva = Array.from(perguntaElemento.querySelectorAll('[data-formulario-modelo="alternativa"]'));

        if (alternativasDaPerguntaObjetiva.length === 0) {
            return false;
        }

        return true;
    },
    /** FIM - Gerenciamento das perguntas do modelo **/

    /** INICIO - Duplicar pergunta **/
    async duplicarPergunta(perguntaId) {
        const perguntaElemento = document.querySelector(`[data-formulario-modelo="pergunta"][data-id="${perguntaId}"]`);

        const perguntaJson = this.criarPerguntaJson(perguntaElemento);

        await this.addPergunta(perguntaJson.secaoId, perguntaJson.tipo, perguntaJson);
    },
    criarPerguntaJson(perguntaElemento) {
        const perguntaJson = {
            tipo: perguntaElemento.querySelector('[name*="Tipo"]')?.value,
            enunciado: perguntaElemento.querySelector('[name*="Enunciado"]')?.value,
            descricao: perguntaElemento.querySelector('[name*="Descricao"]')?.value,
            obrigatorio: perguntaElemento.querySelector('[name*="Obrigatorio"]')?.value === "true",
            secaoId: perguntaElemento.dataset.secaoId
        }

        if (this.isPerguntaDiscursiva(perguntaJson.tipo)) {
            this.addIntervaloCaracteresEmPergunta(perguntaElemento, perguntaJson);
        }

        else if (this.isPerguntaObjetiva(perguntaJson.tipo)) {
            perguntaJson["alternativas"] = [];
            this.addAlternativasEmPergunta(perguntaElemento, perguntaJson);
        }

        if (this.isPerguntaTextoCurto(perguntaJson.tipo)) {
            this.addFormatoTextoEmPerguntaTextoCurto(perguntaElemento, perguntaJson);
        }

        return perguntaJson;
    },
    addIntervaloCaracteresEmPergunta(perguntaElemento, perguntaJson) {
        const quantidadeMinimaCaracteres = perguntaElemento.querySelector('[name*="QuantidadeMinimaCaracteres"]')?.value;
        const quantidadeMaximaCaracteres = perguntaElemento.querySelector('[name*="QuantidadeMaximaCaracteres"]')?.value;

        perguntaJson["quantidadeMinimaCaracteres"] = quantidadeMinimaCaracteres ? Number(quantidadeMinimaCaracteres) : null;
        perguntaJson["quantidadeMaximaCaracteres"] = quantidadeMaximaCaracteres ? Number(quantidadeMaximaCaracteres) : null;
    },
    addAlternativasEmPergunta(perguntaElemento, perguntaJson) {
        const alternativasDaPerguntaElementos = perguntaElemento.querySelectorAll('[data-formulario-modelo="alternativa"]');

        alternativasDaPerguntaElementos.forEach((alternativaElemento) => {
            perguntaJson.alternativas.push({
                texto: alternativaElemento.querySelector('[name*="Texto"]')?.value
            })
        });
    },
    addFormatoTextoEmPerguntaTextoCurto(perguntaElemento, perguntaJson) {
        const formatoTexto = perguntaElemento.querySelector('[name*="FormatoTexto"]')?.value;

        perguntaJson["formatoTexto"] = formatoTexto ? Number(formatoTexto) : null;
    },
    /** FIM - Duplicar pergunta **/

    /** INICIO - Gerenciamento das alternativas das perguntas do modelo **/
    async addAlternativa(perguntaId, secaoId, tipoDaPergunta, alternativaDuplicadaJson = null) {
        try {
            const response = await this.getAlternativaHTML(perguntaId, secaoId, tipoDaPergunta);

            const contentType = response.headers.get('content-type');

            if (contentType && contentType.includes('application/json')) {
                const responseJson = await response.json();
                throw new Error(responseJson.messages.join("\n"));
            }

            const alternativaHTML = await response.text();
            const alternativaElemento = this.converterTextoEmHTML(alternativaHTML).body.firstChild;

            this._renderizarAlternativa(alternativaElemento);

            if (alternativaDuplicadaJson != null) {
                this.preencherAlternativaElementoComValoresDuplicados(alternativaDuplicadaJson, alternativaElemento);
            }
        }

        catch (error) {
            toastr.error(error.message, { timeOut: 3000 });
        }
    },

    async getAlternativaHTML(perguntaId, secaoId, tipoDaPergunta) {
        return await fetch(`${this.urlAlternativa}?perguntaId=${perguntaId}&secaoId=${secaoId}&tipoDaPergunta=${tipoDaPergunta}`, {
            method: 'get',
            headers: {
                "Content-Type": 'application/json'
            }
        });
    },
    _renderizarAlternativa(alternativaElemento) {

        const perguntaId = alternativaElemento.dataset.perguntaId;

        alternativaElemento.querySelector('[name*="Ordem"]').value = this.obterTotalDeAlternativasDaPergunta(perguntaId) + 1;

        const perguntaContainer = document.querySelector(`[data-formulario-modelo="pergunta"][data-id="${perguntaId}"]`)
            .querySelector('[data-formulario-modelo="alternativa-container"]');

        perguntaContainer.appendChild(alternativaElemento);

        this.configurePlugins();

        this.configureValidacoesEmCampos();
    },
    removerAlternativa(alternativaId) {
        const alternativaElemento = document.querySelector(`[data-formulario-modelo="alternativa"][data-id="${alternativaId}"]`);

        const perguntaId = alternativaElemento.dataset.perguntaId;

        alternativaElemento.remove();

        this.atualizarOrdemAlternativasEmPergunta(perguntaId);
    },
    configurarOrdenacaoEmAlternativasDaPergunta(perguntaElemento) {
        this.draggableListaAlternativas.push(new Sortable.default(perguntaElemento.querySelector('[data-formulario-modelo="alternativa-container"]'), {
            draggable: '[data-formulario-modelo="alternativa"]',
            handle: '[data-formulario-modelo="alternativa-handle"]',
            mirror: {
                constrainDimensions: true,
            },

            plugins: [Plugins.SortAnimation],
        })
            .on('drag:start', event => {
                const elementoOriginal = event.source;

                elementoOriginal.style.cssText = 'opacity: 0.6; cursor: move !important';

            })
            .on('drag:stopped', event => {
                const elementoOriginal = event.source;

                const perguntaId = this.obterPerguntaIdEmAlternativa(elementoOriginal);

                this.atualizarOrdemAlternativasEmPergunta(perguntaId);
            }));
    },
    preencherAlternativaElementoComValoresDuplicados(alternativaDuplicadaJson, alternativaElemento) {
        alternativaElemento.querySelector('[name*="Texto"]').value = alternativaDuplicadaJson.texto;
    },
    atualizarOrdemAlternativasEmPergunta(perguntaId) {
        const alternativasDaPergunta = document.querySelector(`[data-formulario-modelo="pergunta"][data-id="${perguntaId}"]`)
            .querySelectorAll('[data-formulario-modelo="alternativa"]');

        alternativasDaPergunta.forEach(function (alternativaElemento, index) {
            alternativaElemento.querySelector("[name*='Ordem']").value = index + 1;
        });
    },
    obterPerguntaIdEmAlternativa(alternativaElemento) {
        return alternativaElemento.dataset.perguntaId;
    },
    obterTotalDeAlternativasDaPergunta(perguntaId) {
        return document.querySelector(`[data-formulario-modelo="pergunta"][data-id="${perguntaId}"]`)
            .querySelectorAll('[data-formulario-modelo="alternativa"]')
            .length;
    },
    exibirButtonRemoverAlternativa(btnRemoverAlternativa) {
        btnRemoverOpcao.classList.remove('d-none');
    },
    esconderButtonRemoverAlternativa(btnRemoverAlternativa) {
        btnRemoverOpcao.classList.add('d-none');
    },
    /** FIM - Gerenciamento das alternativas das perguntas do modelo **/

    /** INICIO - Salvar modelo de formulário **/
    salvar() {
        const formularioForm = document.querySelector('[data-formulario-modelo="formulario"]');

        const formularioFormData = new FormData(formularioForm);

        fetch(formularioForm.getAttribute("action"), {
            method: formularioForm.getAttribute("method"),
            body: formularioFormData,
        }).then(response => response.json())
            .then(response => {
                const messageComplete = response.messages.join("\n");

                if (response.isSuccess === true) {
                    toastr.success(messageComplete, { timeOut: 1500 });
                    setTimeout(() => {
                        window.location.href = response.urlRedirect
                    }, 1500);
                }
                else {
                    throw new Error(messageComplete);
                }
            }).catch(error => {
                toastr.error(error.message, { timeOut: 3000 })
            });
    },
    /** FIM - Salvar modelo de formulário **/

    /** INICIO - Métodos auxiliares **/
    toggleCheckboxValue(checkbox) {
        checkbox.value = checkbox.checked ? true : false;
    },
    converterTextoEmHTML(htmlTexto) {
        const parser = new DOMParser();
        const htmlDocument = parser.parseFromString(htmlTexto, "text/html");
        return htmlDocument;
    },
    /** FIM - Métodos auxiliares **/
}