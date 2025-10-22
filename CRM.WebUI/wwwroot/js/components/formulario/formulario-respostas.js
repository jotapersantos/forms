const formularioRespostas = {
    formularioId: null,
    urlRespostaResumo: null,
    urlPerguntasRespostas: null,
    urlRespostasIndividuais: null,
    urlGerarRelatorio: null,
    urlExclusaoRespostaFormulario: null,
    painelResumoResposta: null,
    painelPerguntaRespostas: null,
    painelRespostasIndividuais: null,
    itemResumoOptionsListagem: [],
    aguardandoRespostas: null,
    secaoItemGraficos: [],
    tokenRequisicao: null,
    /** INICIO - Inicialização e configuração da tela de respostas e abas **/
    inicializar(formularioId, urlRespostaResumo, urlPerguntasRespostas, urlRespostasIndividuais, urlExclusaoRespostaFormulario, urlGerarRelatorio, tokenRequisicao, aguardandoRespostas = false) {
        this.formularioId = formularioId;
        this.urlRespostaResumo = urlRespostaResumo;
        this.urlPerguntasRespostas = urlPerguntasRespostas;
        this.urlRespostasIndividuais = urlRespostasIndividuais;
        this.urlExclusaoRespostaFormulario = urlExclusaoRespostaFormulario;
        this.urlGerarRelatorio = urlGerarRelatorio;
        this.tokenRequisicao = tokenRequisicao;
        this.aguardandoRespostas = aguardandoRespostas;

        if (this.aguardandoRespostas === false)
            this.configurarPaineis();
    },
    configurarPaineis() {
        this.painelResumoResposta = document.querySelector('[data-formulario-resposta="painel-resumo-respostas"]');
        this.painelPerguntaRespostas = document.querySelector('[data-formulario-resposta="painel-pergunta-respostas"]');
        this.painelRespostasIndividuais = document.querySelector('[data-formulario-resposta="painel-respostas-individuais"]');

        const buttonAcessoPaineisLista = document.querySelector('[data-formulario-resposta="button-acesso-paineis"]')
            .querySelectorAll(".nav-link");

        const painelRespostasLista = document.querySelector('[data-formulario-resposta="painel-respostas-container"]')
            .querySelectorAll(".tab-pane");

        buttonAcessoPaineisLista.forEach(function (link) {
            link.addEventListener('shown.bs.tab', function (event) {
                event.preventDefault();

                painelRespostasLista.forEach(function (painel) {
                    painel.classList.remove("active", "show");
                });

                const painelAlvo = this.getAttribute("data-bs-target");
                const painelAlvoElemento = document.querySelector(painelAlvo);


                formularioRespostas.secaoItemGraficos.forEach(function (graficoInstance) {
                    graficoInstance.destroy();
                });

                const tipoPainel = painelAlvoElemento.dataset.formularioResposta;
                switch (tipoPainel) {
                    case 'painel-resumo-respostas':
                        formularioRespostas.abrirResumoRespostas();
                        break;
                    case 'painel-pergunta-respostas':
                        formularioRespostas.abrirPerguntasRespostasPainel(1);
                        break;
                    case 'painel-respostas-individuais':
                        formularioRespostas.abrirRespostasIndividuais(1);
                        break;
                    default:
                        painelAlvoElemento.innerHTML = ' <span>Em Breve!</span>';
                        painelAlvoElemento.classList.add("active", "show");
                        break;
                }
            });
        });
    },
    /** FIM - Inicialização e configuração da tela de respostas e abas **/

    /** INICIO - Painel de resumo de respostas do formulário **/
    abrirResumoRespostas() {
        if (this.aguardandoRespostas === false) {
            fetch(this.urlRespostaResumo, {
                method: "GET"
            }).then(response => response.json())
                .then(response => {
                    if (response.isSuccess === true) {
                        this._renderizarRespostasResumo(response.result);
                    }
                    else {
                        let messageComplete = "";
                        for (let message of response.messages) {
                            messageComplete += message;
                        }
                        throw new Error(messageComplete);
                    }
                }).catch(error => {
                    toastr.error(error, { timeOut: 3000 });
                });
        }
    },
    _renderizarRespostasResumo(responseJson) {
        const formularioSecaoContainer = document.createElement("div");
        formularioSecaoContainer.dataset.formularioResposta = "secoes-container";

        if (responseJson.emailRespostas.length > 0) {
            const cardEmailRespostas = this._criarCardEmailRespostas(responseJson.emailRespostas);
            formularioSecaoContainer.append(cardEmailRespostas);
        }

        const secaoResumos = responseJson.secaoResumos;
        const multiplasSecoes = secaoResumos.length > 1;

        secaoResumos.forEach(function (secaoResumo) {
            const secaoElemento = formularioRespostas._criarSecoesResumo(secaoResumo, multiplasSecoes);
            formularioSecaoContainer.append(secaoElemento);
        });

        this.painelResumoResposta.innerHTML = formularioSecaoContainer.outerHTML;
        this.painelResumoResposta.classList.add("active");
        this.renderizarItemResumoGraficos();
    },
    _criarCardEmailRespostas(emailRespostas) {
        const secaoItemElemento = document.createElement("section");
        secaoItemElemento.classList.add("card", "mb-5");
        secaoItemElemento.innerHTML = `  <header class="card-header">
                                            <div class="card-title m-0">
                                                <h5 class="fw-bold required m-0">Quem respondeu o formulário?</h4>
                                            </div>
                                        </header>
                                        <div class="card-body">                                            
                                            <div class="text-dark fw-bold mb-3">${this._exibirTotalRespostasTexto(emailRespostas.length)}</div>
                                            ${this._criarListaRespostasDiscursivas(emailRespostas).outerHTML}
                                        </div>`;
        return secaoItemElemento;
    },
    _criarSecoesResumo(secaoResumo, multiplasSecoes) {
        const secaoResumoElemento = document.createElement("div");
        secaoResumoElemento.dataset.formularioResposta = "secao-resumo";
        secaoResumoElemento.innerHTML = `${multiplasSecoes === true ? `<h4 class="mb-3">${secaoResumo.titulo}</h4>` : ''}
                                         ${formularioRespostas._criarSecaoItemResumoContainer(secaoResumo.itemResumos).outerHTML}`;
        return secaoResumoElemento;
    },
    _criarSecaoItemResumoContainer(itemResumos) {
        const secaoItemResumoContainer = document.createElement("div");
        secaoItemResumoContainer.dataset.formularioResposta = "secao-item-resumo-container";
        itemResumos.forEach(function (itemResumo) {
            secaoItemResumoContainer.append(formularioRespostas._criarCardSecaoItemResumo(itemResumo));
        });

        return secaoItemResumoContainer;
    },
    _criarCardSecaoItemResumo(itemResumo) {
        const secaoItemElemento = document.createElement("section");
        secaoItemElemento.classList.add("card", "mb-5");
        secaoItemElemento.innerHTML = ` <header class="card-header">
                                            <div class="card-title m-0">
                                                <h5 class="fw-bold required m-0">${itemResumo.pergunta}</h4>
                                            </div>
                                        </header>
                                        <div class="card-body">
                                            <div class="text-dark fw-bold mb-3">${formularioRespostas._exibirTotalRespostasTexto(itemResumo.totalRespostas)}</div>
                                            ${formularioRespostas._criarSecaoItemResumoRespostaPorTipo(itemResumo).outerHTML}
                                        </div>`;
        return secaoItemElemento;
    },
    _exibirTotalRespostasTexto(totalRespostas) {
        if (totalRespostas < 1) {
            return `${totalRespostas} resposta`;
        }
        return `${totalRespostas} respostas`;
    },
    _criarSecaoItemResumoRespostaPorTipo(itemResumo) {
        switch (itemResumo.tipo) {
            case "TextoCurto":
            case "TextoLongo":
                return formularioRespostas._criarListaRespostasDiscursivas(itemResumo.textoRespostas);
            case "MultiplaEscolha":
            case "ListaSuspensa":
                return formularioRespostas._criarGraficoPizzaRespostasObjetivas(itemResumo.id, itemResumo.opcaoTextos, itemResumo.opcaoTotalRespostas);
            case "CaixaSelecao":
                return formularioRespostas._criarGraficoBarraRespostasObjetivas(itemResumo.id, itemResumo.opcaoTextos, itemResumo.opcaoTotalRespostas, itemResumo.totalRespostas);
            default:
                return document.createElement("div");
        }
    },
    _criarListaRespostasDiscursivas(textoRespostas) {
        const textoRespostaListagem = document.createElement("div");
        textoRespostaListagem.classList.add("row");

        if (textoRespostas.length > 0) {
            textoRespostas.forEach(function (textoResposta) {
                const textoRespostaLabel = document.createElement("div");
                textoRespostaLabel.classList.add("col-12");
                textoRespostaLabel.innerHTML = `<div class="border rounded-1 mb-1 p-2" style="background: #f6f6f6">${textoResposta}</div>`;
                textoRespostaListagem.appendChild(textoRespostaLabel);
            });
        }
        else {
            return formularioRespostas.gerarMensagemSemRespostasRegistradasItem();
        }

        return textoRespostaListagem;
    },
    _criarGraficoPizzaRespostasObjetivas(secaoItemId, opcaoTextos, opcaoTotalRespostas) {
        if (formularioRespostas.secaoItemTemOpcoesSemResposta(opcaoTotalRespostas) === true) {
            return formularioRespostas.gerarMensagemSemRespostasRegistradasItem();
        }

        const itemResumoGraficoContainer = document.createElement("div");

        itemResumoGraficoContainer.classList.add("d-flex", "justify-content-center", "mb-3");

        itemResumoGraficoContainer.dataset.formularioResposta = `secao-item-resumo-grafico-container-${secaoItemId}`;

        const itemResumoOptions = {
            id: `secao-item-resumo-grafico-container-${secaoItemId}`,
            series: opcaoTotalRespostas,
            chart: {
                width: '600',
                height: '600',
                type: 'pie',
            },
            labels: opcaoTextos,
            legend: {
                position: 'right'
            },
            plotOptions: {
                pie: {
                    dataLabels: {
                        offset: -10
                    }
                }
            },
            tooltip: {
                y: {
                    formatter: function (val, opts) {
                        const serie = opts.globals.series[opts.seriesIndex];
                        const percentage = Number(opts.globals.seriesPercent[opts.seriesIndex]);
                        return `${serie} (${percentage.toFixed(2)}%)`;
                    }
                }
            },
            responsive: [{
                breakpoint: 320,
                options: {
                    chart: {
                        width: '100%',
                        height: '100%',
                    },
                    legend: {
                        position: 'bottom'
                    }
                },
                breakpoint: 512,
                options: {
                    chart: {
                        width: '100%',
                        height: '100%',
                    },
                    legend: {
                        position: 'bottom'
                    }
                }
            }, {
                breakpoint: 992,
                options: {
                    chart: {
                        width: '100%',
                        height: '100%',
                    },
                    legend: {
                        position: 'bottom'
                    }
                }
            }]
        }
        formularioRespostas.itemResumoOptionsListagem.push(itemResumoOptions);
        return itemResumoGraficoContainer;
    },
    _criarGraficoBarraRespostasObjetivas(secaoItemId, opcaoTextos, opcaoTotalRespostas, totalRespostasItem) {
        if (formularioRespostas.secaoItemTemOpcoesSemResposta(opcaoTotalRespostas) === true) {
            return formularioRespostas.gerarMensagemSemRespostasRegistradasItem();
        }

        const itemResumoGraficoContainer = document.createElement("div");

        itemResumoGraficoContainer.classList.add("d-flex", "justify-content-center", "mb-3");

        itemResumoGraficoContainer.dataset.formularioResposta = `secao-item-resumo-grafico-container-${secaoItemId}`;

        const itemResumoOptions = {
            id: `secao-item-resumo-grafico-container-${secaoItemId}`,
            chart: {
                type: 'bar',
                width: 1000,
                toolbar: {
                    show: false
                },
                height: 400,
            },
            series: [{ data: opcaoTotalRespostas }],
            plotOptions: {
                bar: {
                    horizontal: true,
                    borderRadius: 4,
                    dataLabels: {
                        position: 'top',
                    },
                    barHeight: opcaoTotalRespostas.length > 2 ? '50%' : '30%'
                }
            },
            dataLabels: {
                enabled: true,
                formatter: function (val, { seriesIndex, dataPointIndex, w }) {
                    let porcentagemRespostas = (w.globals.series[seriesIndex][dataPointIndex] / totalRespostasItem) * 100;
                    return `${val.toFixed(0)} (${porcentagemRespostas.toFixed(1)}%)`;
                },
                offsetX: 45,
                offsetY: 0,
                style: {
                    fontSize: '10pt',
                    colors: ['#000']
                }
            },
            xaxis: {
                categories: opcaoTextos,
                max: totalRespostasItem + 1,
                tickAmount: Math.ceil(totalRespostasItem / 8) + 1,
                labels: {
                    formatter: function (value) {
                        return value.toFixed(0);
                    }
                }
            },
            yaxis: {
                labels: {
                    formatter: function (value) {
                        return value;
                    }
                }
            },
            tooltip: {
                y: {
                    title: {
                        formatter: function (series) {
                            return "Contagem: ";
                        }
                    }
                },
            },
            responsive: [
                {
                    breakpoint: 1100,
                    options: {
                        chart: {
                            width: 400,
                            height: 300,
                        },
                        dataLabels: {
                            style: {
                                colors: ['#000'],
                                fontSize: '6pt'
                            }
                        },
                    }
                }]
        }

        formularioRespostas.itemResumoOptionsListagem.push(itemResumoOptions);

        return itemResumoGraficoContainer;
    },
    renderizarItemResumoGraficos() {
        const itemResumoGraficoContainer = document.querySelectorAll('[data-formulario-resposta^="secao-item-resumo-grafico-container"]');
        itemResumoGraficoContainer.forEach(function (graficoContainer) {
            const identificador = graficoContainer.dataset.formularioResposta;
            const graficoOptions = formularioRespostas.itemResumoOptionsListagem.find(function (grafico) {
                return grafico.id === identificador;
            });
            const itemResumoGrafico = new ApexCharts(graficoContainer, graficoOptions);
            itemResumoGrafico.render();
            formularioRespostas.secaoItemGraficos.push(itemResumoGrafico);
        });
    },
    gerarMensagemSemRespostasRegistradasItem() {
        const mensagemSemRespostasElemento = document.createElement("div");
        mensagemSemRespostasElemento.classList.add("row");
        mensagemSemRespostasElemento.innerHTML = `<div class="col-12">
                                                    <span class="py-2"> Não há respostas registradas para esta pergunta.</span>
                                                </div>`;
        return mensagemSemRespostasElemento;
    },
    secaoItemTemOpcoesSemResposta(opcaoTotalRespostas) {
        return opcaoTotalRespostas.every(function (totalResposta) {
            return totalResposta === 0;
        })
    },
    /** FIM - Painel de resumo de respostas do formulário **/

    /** INICIO - Painel de exibição de respostas do formulário por pergunta **/
    abrirPerguntasRespostasPainel(pagina) {
        fetch(`${this.urlPerguntasRespostas}?page=${pagina}`, {
            method: "GET",
            headers: {
                "Content-Type": "text/html"
            }
        }).then(response => response.text())
            .then(response => {
                formularioRespostas._renderizarPerguntasRespostasPainel(response);
            });
    },
    _renderizarPerguntasRespostasPainel(responseHTML) {
        formularioRespostas.painelPerguntaRespostas.innerHTML = responseHTML;
        formularioRespostas.painelPerguntaRespostas.classList.add("active", "show");
    },
    /** INICIO - Painel de exibição de respostas do formulário por pergunta **/

    /** INICIO - Painel de exibição de respostas do formulário por registro **/
    abrirRespostasIndividuais(pagina) {
        fetch(`${this.urlRespostasIndividuais}?page=${pagina}`, {
            method: "GET",
            headers: {
                "Content-Type": "text/html"
            }
        }).then(response => response.text())
            .then(response => {
                formularioRespostas._renderizarRespostasIndividuais(response);
            });
    },
    _renderizarRespostasIndividuais(responseHTML) {
        formularioRespostas.painelRespostasIndividuais.innerHTML = responseHTML;
        formularioRespostas.painelRespostasIndividuais.classList.add("active", "show");
    },
    excluirRespostaDoFormulario(respostaId) {
        Swal.fire({
            title: "Você tem certeza que deseja excluir esta resposta?",
            text: "Cuidado! Esse processo é irreversível.",
            icon: "question",
            buttonsStyling: false,
            showCancelButton: true,
            confirmButtonText: "Sim, excluir!",
            cancelButtonText: "Não, cancelar.",
            customClass: {
                confirmButton: "btn btn-danger",
                cancelButton: "btn btn-primary"
            }
        }).then((result) => {
            if (result.isConfirmed) {
                const url = formularioRespostas.urlExclusaoRespostaFormulario.replace("00000000-0000-0000-0000-000000000000", respostaId);

                fetch(url, {
                    method: "POST",
                    headers: {
                        "RequestVerificationToken": formularioRespostas.tokenRequisicao
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
                                window.location.reload();
                            }, 1500);
                        } else {
                            throw new Error(messageComplete);
                        }
                    }).catch(error => {
                        toastr.error(error, { timeOut: 3000 });
                    })
            }
        });
    },
    /** FIM - Painel de exibição de respostas do formulário por registro **/

    /** INICIO - Geração de relatórios de formulário em CSV **/
    obterRelatorioFormulario() {
        fetch(this.urlGerarRelatorio, {
            method: 'GET'
        }).then(response => response.json())
            .then(response => {
                if (response.isSuccess === true) {
                    this.gerarArquivoRelatorioFormulario(response.result);
                }
                else {
                    let messageComplete = "";
                    for (let message of response.messages) {
                        messageComplete += message;
                    }
                    throw new Error(messageComplete);
                }
            }).catch(error => {
                toastr.error(error, { timeOut: 3000 });
            });
    },
    gerarArquivoRelatorioFormulario(relatorioArquivo) {
        const contentType = relatorioArquivo.contentType;
        const fileContents = relatorioArquivo.fileContents;
        const fileDownloadName = relatorioArquivo.fileDownloadName;
        const uint8Array = Uint8Array.from(atob(fileContents), c => c.charCodeAt(0)).buffer;
        const relatorioArquivoBlob = new Blob([uint8Array], { type: contentType });
        const relatorioArquivoFile = new File([relatorioArquivoBlob], fileDownloadName, { type: contentType });
        const link = document.createElement("a");
        link.href = window.URL.createObjectURL(relatorioArquivoFile);
        link.download = fileDownloadName;
        link.click();
    }
    /** FIM - Geração de relatórios de formulário em CSV **/
}
