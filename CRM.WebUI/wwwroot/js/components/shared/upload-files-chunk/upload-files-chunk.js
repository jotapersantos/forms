class UploadFilesChunk {
    constructor(
        urlIniciarUploadMultipart,
        urlUploadPart,
        urlCompleteUploadMultipart,
        extensoesPermitidas,
        tamanhoPermitidoArquivo,
    ) {
        this.urlIniciarUploadMultipart = urlIniciarUploadMultipart;
        this.urlUploadPart = urlUploadPart;
        this.urlCompleteUploadMultipart = urlCompleteUploadMultipart;
        this.extensoesPermitidas = extensoesPermitidas;
        this.tamanhoPermitidoArquivo = tamanhoPermitidoArquivo;
        this.callbackFinalizar = null;
        this.modalUploadMultipartFile = null;
    }

    adicionarCallbackFinalizar(callbackFinalizar) {
        this.callbackFinalizar = callbackFinalizar;
    }

    async initUploadMultipart(fileUpload, callbackErro = null, fileServerPath = "") {
        this.exibirModal();

        const fileWithSanitizedName = new File([fileUpload], this.obterNomeArquivoNormalizado(fileUpload.name), {
            type: fileUpload.type,
            lastModified: fileUpload.lastModified
        });
        ;

        const uploadMultipartResponse = await fetch(this.urlIniciarUploadMultipart, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                arquivoNome: fileWithSanitizedName.name,
                arquivoTamanho: fileWithSanitizedName.size,
                extensoesPermitidas: this.extensoesPermitidas,
                tamanhoPermitidoArquivo: this.tamanhoPermitidoArquivo,
                arquivoCaminho: fileServerPath
            })
        }).then(response => response.json())
            .then(response => {
                if (response.success === true) {
                    return {
                        uploadId: response.uploadId,
                        keyFileId: response.keyFileId,
                        fileWithKey: response.fileWithKey
                    }
                }
                let messages = `Alguns erros de validação foram encontrados: ${response.messages.join("\n")}`;
                toastr.error(messages, { timeOut: 3000 });
                return null;
            });

        if (uploadMultipartResponse !== null) {
            const uploadPartETags = [];
            let contadorProgresso = 0;

            const uploader = new qq.FineUploaderBasic({
                request: {
                    endpoint: this.urlUploadPart, // URL do endpoint do backend para receber chunks
                    method: "POST",
                    params: {
                        uploadId: uploadMultipartResponse.uploadId,
                        keyFileId: uploadMultipartResponse.keyFileId,
                        filePath: fileServerPath
                    }
                },
                validation: {
                    sizeLimit: 10 * 1024 * 1024 * 1024,
                    allowedExtensions: this.obterExtensoesSemPontuacao(),
                },
                messages: {
                    sizeError: "O vídeo enviado excede o tamanho permitido. Tamanho máximo permitido de 10GB",
                    typeError: "Extensão de arquivo não permitida. Extensões permitidas: {extensions}",
                },
                chunking: {
                    enabled: true,
                    partSize: 5 * 1024 * 1024,
                    concurrent: {
                        enabled: true
                    }
                },
                maxConnections: 100,
                retry: {
                    enableAuto: true
                },
                resume: {
                    enable: true
                },
                multiple: false,
                callbacks: {
                    onComplete: async (id, fileName, response) => {
                        if (response.partNumber === 1) {
                            $('[data-upload-files-chunk="progress-bar"]').css('width', 100 + '%');
                            $('[data-upload-files-chunk="progress-bar"]').text(100 + '%');
                            uploadPartETags.push({
                                partNumber: response.partNumber,
                                eTag: response.eTag
                            });
                        }

                        const location = await fetch(this.urlCompleteUploadMultipart,
                            {
                                method: 'POST',
                                headers: {
                                    "Content-Type": "application/json"
                                },
                                body: JSON.stringify({
                                    uploadId: uploadMultipartResponse.uploadId,
                                    arquivoNome: `${uploadMultipartResponse.keyFileId}_${fileName}`,
                                    uploadPartETags: uploadPartETags,
                                    arquivoCaminho: fileServerPath
                                })
                            }).then(response => response.json())
                            .then(response => {
                                return response.urlLocation
                            });

                        this.esconderModal();

                        this.callbackFinalizar(location);
                    },
                    onUploadChunkSuccess: (id, chunkData, responseJSON) => {
                        if (responseJSON.success === true) {
                            contadorProgresso++;
                            const envioArquivoPercentual = (contadorProgresso / responseJSON.totalParts) * 100;
                            $('[data-upload-files-chunk="progress-bar"]').css('width', envioArquivoPercentual.toFixed(2) + '%');
                            $('[data-upload-files-chunk="progress-bar"]').text(envioArquivoPercentual.toFixed(2) + '%');
                            uploadPartETags.push({
                                partNumber: responseJSON.partNumber,
                                eTag: responseJSON.eTag
                            });
                        }
                    },
                    onError: (id, name, errorReason, xhr) => {
                        if (errorReason !== 'No files to upload.') {
                            toastr.error(errorReason, { timeOut: 1500 });

                            if (callbackErro) {
                                callbackErro();
                            }
                            this.esconderModal();
                        }
                    }
                },
                autoUpload: false
            });

            uploader.addFiles(fileWithSanitizedName);
            uploader.uploadStoredFiles();
        }
        else {

            if (callbackErro) {
                callbackErro();
            }
            this.esconderModal();
        }
    }

    async initUploadMultipartComRetornoDoLocation(fileUpload, fileServerPath = "") {

        //Cria um arquivo com o nome sem caracteres especiais ou espaços
        const fileWithSanitizedName = new File([fileUpload], this.obterNomeArquivoNormalizado(fileUpload.name), {
            type: fileUpload.type,
            lastModified: fileUpload.lastModified
        });
        ;

        //Cria um InitUploadRequest do S3 para iniciar o envio multipart
        const uploadMultipartResponse = await fetch(this.urlIniciarUploadMultipart, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                arquivoNome: fileWithSanitizedName.name,
                arquivoTamanho: fileWithSanitizedName.size,
                extensoesPermitidas: this.extensoesPermitidas,
                tamanhoPermitidoArquivo: this.tamanhoPermitidoArquivo,
                arquivoCaminho: fileServerPath
            })
        }).then(response => response.json())
            .then(response => {
                if (response.success === true) {
                    return {
                        uploadId: response.uploadId,
                        keyFileId: response.keyFileId,
                        fileWithKey: response.fileWithKey
                    }
                }
                let messages = `Alguns erros de validação foram encontrados: ${response.messages.join("\n")}`;
                toastr.error(messages, { timeOut: 3000 });
                return null;
            });

        //Inicia o processo do upload multipart do arquivo e depois devolve o location caso o envio tenha sido bem sucedido.
        if (uploadMultipartResponse !== null) {
            const uploadPartETags = [];

            return new Promise((resolve, reject) => {
                const uploader = new qq.FineUploaderBasic({
                    request: {
                        endpoint: this.urlUploadPart, // URL do endpoint do backend para receber chunks
                        method: "POST",
                        params: {
                            uploadId: uploadMultipartResponse.uploadId,
                            keyFileId: uploadMultipartResponse.keyFileId,
                            filePath: fileServerPath
                        }
                    },
                    validation: {
                        sizeLimit: 10 * 1024 * 1024 * 1024,
                        allowedExtensions: this.obterExtensoesSemPontuacao(),
                    },
                    messages: {
                        sizeError: "O arquivo enviado excede o tamanho permitido. Tamanho máximo permitido de 10GB",
                        typeError: "Extensão de arquivo não permitida. Extensões permitidas: {extensions}",
                    },
                    chunking: {
                        enabled: true,
                        partSize: 5 * 1024 * 1024,
                        concurrent: {
                            enabled: true
                        }
                    },
                    maxConnections: 100,
                    retry: {
                        enableAuto: true
                    },
                    resume: {
                        enable: true
                    },
                    multiple: false,
                    callbacks: {
                        onComplete: async (id, fileName, response) => {
                            if (response.partNumber === 1) {
                                uploadPartETags.push({
                                    partNumber: response.partNumber,
                                    eTag: response.eTag
                                });
                            }

                            const location = await fetch(this.urlCompleteUploadMultipart,
                                {
                                    method: 'POST',
                                    headers: {
                                        "Content-Type": "application/json"
                                    },
                                    body: JSON.stringify({
                                        uploadId: uploadMultipartResponse.uploadId,
                                        arquivoNome: `${uploadMultipartResponse.keyFileId}_${fileName}`,
                                        uploadPartETags: uploadPartETags,
                                        arquivoCaminho: fileServerPath
                                    })
                                }).then(response => response.json())
                                .then(response => {
                                    return response.urlLocation
                                });

                            this.esconderModal();

                            resolve(location);
                        },
                        onUploadChunkSuccess: (id, chunkData, responseJSON) => {
                            if (responseJSON.success === true) {
                                uploadPartETags.push({
                                    partNumber: responseJSON.partNumber,
                                    eTag: responseJSON.eTag
                                });
                            }
                        },
                        onError: (id, name, errorReason, xhr) => {
                            reject(errorReason);
                        }
                    },
                    autoUpload: false
                });

                uploader.addFiles(fileWithSanitizedName);
                uploader.uploadStoredFiles();
            });
        }
        else {
            return null;
        }
    }

    obterExtensoesSemPontuacao() {
        return this.extensoesPermitidas.map((extensao) => {
            const extensaoFormatada = extensao.replace(".", "");
            return extensaoFormatada;
        });
    }

    obterNomeArquivoNormalizado(arquivoNome) {
        return arquivoNome.replace(/[^a-zA-Z0-9.-]/g, "-");
    }

    exibirModal() {
        this.modalUploadMultipartFile = new bootstrap.Modal(document.querySelector('[data-upload-files-chunk="modal-upload"]'), {
            backdrop: 'static',
            keyboard: false
        });

        this.modalUploadMultipartFile.show();
    }

    esconderModal() {
        if (this.modalUploadMultipartFile) {
            this.modalUploadMultipartFile.hide();
            this.modalUploadMultipartFile = null;
        }
    }
}
