const cursoSort = {
    urlSalvarOrdenacaoCursos: null,
    tokenRequisicao: null,
    inicializar(urlSalvarOrdenacaoCursos, tokenRequisicao) {
        this.urlSalvarOrdenacaoCursos = urlSalvarOrdenacaoCursos;
        this.tokenRequisicao = tokenRequisicao;
        this.configurarDraggableEmContainerCursos();
    },
    configurarDraggableEmContainerCursos() {
        const cursosContainer = document.querySelector('[data-curso-sort="container"]');

        new Sortable.default(cursosContainer, {
            draggable: '[data-curso-sort="item"]',
            mirror: {
                appendTo: "body",
                constrainDimensions: true
            },
        }).on('drag:stopped', event => {
                this.atualizarOrdemCursos();
            });
    },
    atualizarOrdemCursos() {
        const cursoItensElementos = document.querySelectorAll('[data-curso-sort="item"]');
        cursoItensElementos.forEach((cursoElemento, index) => {
            cursoElemento.dataset.cursoSortOrdem = index + 1;
        });
    },
    salvarOrdemCursos() {
        const cursoItensElementos = Array.from(document.querySelectorAll('[data-curso-sort="item"]'));
        const cursoIdsComOrdens = cursoItensElementos.map(cursoElemento => {
            return {
                cursoId: cursoElemento.dataset.cursoSortId,
                ordem: Number(cursoElemento.dataset.cursoSortOrdem)
            }
        });

        fetch(this.urlSalvarOrdenacaoCursos, {
            method: "POST",
            body: JSON.stringify(cursoIdsComOrdens),
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