const viewSortItems = {
    urlSalvarOrdenacaoItens: null,
    tokenRequisicao: null,
    inicializar(urlSalvarOrdenacaoItens, tokenRequisicao) {
        this.urlSalvarOrdenacaoItens = urlSalvarOrdenacaoItens;
        this.tokenRequisicao = tokenRequisicao;
        this.configurarDraggableEmContainerItens();
    },
    configurarDraggableEmContainerItens() {
        const viewSortItensContainer = document.querySelector('[data-view-sort-items="container"]');

        new Sortable.default(viewSortItensContainer, {
            draggable: '[data-view-sort-items="item"]',
            mirror: {
                appendTo: '[data-view-sort-items="container"]',
                constrainDimensions: true
            },
        }).on('drag:stopped', event => {
                this.atualizarOrdemItens();
            });
    },
    atualizarOrdemItens() {
        const viewSortItemsElementos = document.querySelectorAll('[data-view-sort-items="item"]');
        viewSortItemsElementos.forEach((elemento, index) => {
            elemento.dataset.viewSortItemsOrdem = index + 1;
        });
    },
    salvarOrdemItens() {
        const viewSortItemsElemento = Array.from(document.querySelectorAll('[data-view-sort-items="item"]'));
        const itensIdsComOrdens = viewSortItemsElemento.map(elemento => {
            return {
                id: elemento.dataset.viewSortItemsId,
                ordem: Number(elemento.dataset.viewSortItemsOrdem)
            }
        });

        fetch(this.urlSalvarOrdenacaoItens, {
            method: "POST",
            body: JSON.stringify(itensIdsComOrdens),
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