const modalDetalhesEvento = {
    urlCarregarDetalhesEventoModal: null,
    detalhesEventoModalElemento: null,
    detalhesEventoModalInstance: null,
    acaoAbrirModalEditarEvento: false,
    acaoExcluirEvento: false,
    inicializar(urlCarregarDetalhesEventoModal) {
        this.urlCarregarDetalhesEventoModal = urlCarregarDetalhesEventoModal;
        this.detalhesEventoModalElemento = document.querySelector("#detalhes_evento_modal");
    },
    abrir(eventoId) {
        let url = `${this.urlCarregarDetalhesEventoModal}`.replace("00000000-0000-0000-0000-000000000000", eventoId);
        fetch(url, {
            method: 'GET',
        }).then(response => response.text())
            .then(response => {   

                modalDetalhesEvento.detalhesEventoModalInstance = new bootstrap.Modal(modalDetalhesEvento.detalhesEventoModalElemento);
                modalDetalhesEvento.detalhesEventoModalElemento.querySelector(".modal-content").innerHTML = response;
                modalDetalhesEvento.detalhesEventoModalInstance.show();
            });
    },
    abrirModalEditarEvento(eventoId) {
        modalDetalhesEvento.detalhesEventoModalInstance.hide();
        modalEditarEvento.abrir(eventoId);
    }
}