const modalOrdernarPerguntasModelo = {
    modal: null,
    callback: null,
    secaoId: null,
    abrir(listaPerguntas, secaoId, callbackAoSalvar) {
        const modalHTML = this.getModalElemento(listaPerguntas);

        this.callback = callbackAoSalvar;

        this.secaoId = secaoId;

        const modalContainer = document.createElement("div");
        modalContainer.innerHTML = modalHTML;

        document.body.appendChild(modalContainer);

        this.modal = new bootstrap.Modal(document.querySelector('[data-modal-ordernar-perguntas="modal"]'));

        this.modal.show();

        this.initDraggablePerguntas();

        this.initBtnSalvarOrdem();

        modalContainer.addEventListener('hidden.bs.modal', () => {
            document.body.removeChild(modalContainer);
        });
    },
    getModalElemento(listaPerguntas) {
        const modalElemento = `<div data-modal-ordernar-perguntas="modal" class="modal fade" tabindex="-1" data-modal="modal">
                                    <div class="modal-dialog modal-lg">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <h2 class="modal-title">Ordernar Perguntas da Seção</h2>
                                                <div class="btn btn-icon btn-active-light-primary" data-bs-dismiss="modal" aria-label="Close">
                                                    <i class="fa-light fa-xmark fs-1"></i>
                                                </div>
                                            </div>

                                            <div class="modal-body">
                                               ${this.gerarListaPerguntaElementos(listaPerguntas).outerHTML}
                                            </div>

                                            <div class="modal-footer">
                                                <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
                                                <button type="button" data-modal-ordenar-perguntas="btn-salvar-ordem" class="btn btn-primary">Salvar</button>
                                            </div>
                                        </div>
                                    </div>
                               </div>`;

        return modalElemento;
    },
    gerarListaPerguntaElementos(listaPerguntasJson) {
        const listaPerguntaElementos = document.createElement("div");
        listaPerguntaElementos.classList.add("d-flex", "flex-column");
        listaPerguntaElementos.setAttribute("data-modal-ordernar-perguntas", "container");

        listaPerguntasJson.forEach((pergunta) => {
            const perguntaTitulo = pergunta.titulo ? pergunta.titulo : pergunta.tipo;
            const perguntaElemento = document.createElement("div");

            perguntaElemento.setAttribute("data-modal-ordenar-perguntas", "pergunta");
            perguntaElemento.dataset.perguntaId = pergunta.id;
            perguntaElemento.classList.add("card", "card-pergunta-item", "mb-3", "shadow-sm");
            perguntaElemento.style.cursor = "grab";
            perguntaElemento.innerHTML = `<div class="card-body">
                                             ${perguntaTitulo}
                                         </div>`;

            listaPerguntaElementos.appendChild(perguntaElemento);
        });

        return listaPerguntaElementos;
    },
    initDraggablePerguntas() {
        const perguntasContainer = document.querySelector('[data-modal-ordernar-perguntas="container"]');

        new Sortable.default(perguntasContainer, {
            draggable: '[data-modal-ordenar-perguntas="pergunta"]',
            mirror: {
                appendTo: perguntasContainer,
                constrainDimensions: true,
            },
            plugins: [Plugins.SortAnimation],
        })
            .on('drag:start', event => {
                const elementoOriginal = event.source;

                elementoOriginal.style.cssText = 'opacity: 0.6; cursor: grabbing !important';
            })
            .on('drag:stopped', event => {
                const elementoOriginal = event.source;

            });
    },
    initBtnSalvarOrdem(callback) {
        const btnSalvarOrdem = document.querySelector('[data-modal-ordenar-perguntas="btn-salvar-ordem"]');

        btnSalvarOrdem.onclick = this.salvar.bind(this);
    },
    salvar() {
        const listaPerguntaIds = Array.from(document.querySelectorAll('[data-modal-ordenar-perguntas="pergunta"]'))
            .map(perguntaElemento => perguntaElemento.dataset.perguntaId);

        this.modal.hide();

        this.callback(this.secaoId, listaPerguntaIds);
    }
}