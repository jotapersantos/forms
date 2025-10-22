const modalPreviewModelo = {
    abrir(urlModal) {
        fetch(urlModal, {
            headers: {
                'Content-Type': 'text/html'
            }
        }).then(response => response.text())
            .then(responseText => {
                const modalElemento = document.querySelector('[data-modal="modal"]');

                modalElemento.querySelector(".modal-content").innerHTML = responseText;

                const modalInstance = new bootstrap.Modal(modalElemento);

                modalInstance.show();

            });
    },        
}