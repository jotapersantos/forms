const carregarGabaritoAvaliado = {
    urlCarregarModeloComGabarito: null,
    container: null,
    inicializar(urlCarregarModeloComGabarito, modeloId) {
        this.urlCarregarModeloComGabarito = urlCarregarModeloComGabarito;
        this.modeloId = modeloId;
        this.container = document.querySelector('[data-carregar-gabarito-avaliado="container"]');
    },
    carregarGabaritoPorAvaliadoSelecionado(selectAvaliados) {
        const gabaritoDoAvaliadoId = selectAvaliados.value;

        if (!gabaritoDoAvaliadoId) {
            this.container.innerHTML = "";
            return;
        }

        let url = this.urlCarregarModeloComGabarito.replace("00000000-0000-0000-0000-000000000000", gabaritoDoAvaliadoId);
        url = `${url}?modeloId=${this.modeloId}`;

        fetch(url, {
            method: 'GET',
            headers: {
                "Content-Type": "text/html"
            }
        }).then(response => response.text())
            .then(responseHtml => {
                this._renderizarGabarito(responseHtml);
            }).catch(error => {
                toastr.error(error, { timeOut: 3000 });
            })
    },
    _renderizarGabarito(html) {
        this.container.innerHTML = html;
    }
}