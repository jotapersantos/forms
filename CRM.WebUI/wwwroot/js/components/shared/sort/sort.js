const sort = {
    urlSalvarOrdenacao: null,
    tokenRequisicao: null,
    inicializar(urlSalvarOrdenacao, tokenRequisicao) {
        this.urlSalvarOrdenacao = urlSalvarOrdenacao;
        this.tokenRequisicao = tokenRequisicao;
        this.configurarDraggableEmContainer();
    },
    configurarDraggableEmContainer() {
        const container = document.querySelector('[data-sort="container"]');

        new Sortable.default(container, {
            draggable: '[data-sort="item"]',
            mirror: {
                appendTo: "body",
                constrainDimensions: true
            },
        }).on('drag:stopped', event => {
                this.atualizarOrdem();
            });
    },
    atualizarOrdem() {
        const itensElementos = document.querySelectorAll('[data-sort="item"]');
        itensElementos.forEach((elemento, index) => {
            elemento.dataset.sortOrdem = index + 1;
        });
    },
    salvarOrdem() {
        const itensElementos = Array.from(document.querySelectorAll('[data-sort="item"]'));
        const itensOrdenados = itensElementos.map(elemento => {
            return {
                id: elemento.dataset.sortId,
                ordem: Number(elemento.dataset.sortOrdem)
            }
        });

        fetch(this.urlSalvarOrdenacao, {
            method: "POST",
            body: JSON.stringify(itensOrdenados),
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': this.tokenRequisicao
            }
        }).then(response => response.json())
            .then(response => {
                let messageComplete = "";
                for (let message of response.messages) {
                    messageComplete += message;
                }
                if (response.isSuccess === true) {
                    toastr.success(messageComplete, { timeOut: 1500 });
                    setTimeout(() => {
                        window.location.href = response.urlRedirect
                    }, 1500);
                }
                else {
                    toastr.error(messageComplete, { timeOut: 3000 });
                }
            });
    }
}