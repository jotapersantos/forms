const agendaCalendario = {
    urlCarregarDadosEventosAgenda: null,
    urlCarregarModalDetalhesEvento: null,
    urlCarregarModalEditarEvento: null,
    urlExcluirEvento: null,
    agendaSelecionadaAtualId: null,
    calendarioContainer: null, 
    calendarioInstance: null,

    inicializar(urlCarregarDadosEventosAgenda, urlCarregarModalCriarEvento, urlCarregarModalDetalhesEvento, urlCarregarModalEditarEvento, urlExcluirEvento, usuarioAtualId) {
        this.urlCarregarDadosEventosAgenda = urlCarregarDadosEventosAgenda;
        this.urlCarregarModalCriarEvento = urlCarregarModalCriarEvento;
        this.urlCarregarModalDetalhesEvento = urlCarregarModalDetalhesEvento;
        this.urlCarregarModalEditarEvento = urlCarregarModalEditarEvento;
        this.urlExcluirEvento = urlExcluirEvento;
        this.usuarioAtualId = usuarioAtualId;
        this.calendarioContainer = document.querySelector("#kt_partial_calendario");

    },
    renderizar() {
        this.configurarSeletorAgenda();
    },
    configurarSeletorAgenda() {
        const optionFormat = function (item) {
            if (!item.id) {
                return item.text;
            }

            var span = document.createElement('span');
            var badgeText = item.element.getAttribute("data-badge-text");
            var badgeColor = item.element.getAttribute("data-badge-color");
            var template = '';

            template += item.text;

            template += `  <span class="badge ${badgeColor}">${badgeText}</span>`;
            span.innerHTML = template;

            return $(span);
        }

        $('[data-lista-agendas]').select2({
            templateSelection: optionFormat,
            templateResult: optionFormat
        });

        this.agendaSelecionadaAtualId = document.querySelector("[data-lista-agendas]").value;
        this.obterDadosEventoParaRenderizarCalendario();
    },
    alterarAgendaExibida(agendaSelecionadaId) {
        this.agendaSelecionadaAtualId = agendaSelecionadaId;
        this.obterDadosEventoParaRenderizarCalendario();
    },
    obterDadosEventoParaRenderizarCalendario() {
        let url = `${this.urlCarregarDadosEventosAgenda}?agendaId=${this.agendaSelecionadaAtualId}`;
        fetch(url, {
            method: 'GET',
        }).then(response => response.json())
            .then(response => {
                this.renderizarCalendario(response);
            });
    },
    renderizarCalendario(eventosDados) {
        const addNovoEventoBtn = {
            novoEventoBtn: {
                text: 'Novo Evento',
                click: function () {
                    modalCriarEvento.abrir(agendaCalendario.agendaSelecionadaAtualId);
                }
            }
        }

        const gerarImagemBtn = {
            gerarImagemBtn: {
                text: 'Gerar Imagem',
                click: function () {
                    agendaCalendario.gerarImagemCalendario();
                },
            }
        }

        const listaBotoesLadoDireitoCalendario = eventosDados.usuarioAtualParticipanteAgenda === true ? 'dayGridMonth,timeGridWeek,timeGridDay gerarImagemBtn novoEventoBtn' : 'dayGridMonth,timeGridWeek,timeGridDay gerarImagemBtn'

        var dataHoje = moment().format("YYYY-MM-DD");
        agendaCalendario.calendarioInstance = new FullCalendar.Calendar(agendaCalendario.calendarioContainer, {
            initialView: 'dayGridMonth',
            locale: 'pt-br',
            initialDate: dataHoje,
            contentHeight: 890,
            eventOrderStrict: true,
            selectable: true,
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: listaBotoesLadoDireitoCalendario
            },
            customButtons: {
                ...addNovoEventoBtn,
                ...gerarImagemBtn
            },
            events: eventosDados.eventos,
            eventClick: function (event, jsEvent, view) {
                modalDetalhesEvento.abrir(event.event.id);
            },
            select: function (diaSelecionado) {
                agendaCalendario.toggleSelecionarDataEmCalendario(false);
                if (eventosDados.usuarioAtualParticipanteAgenda === true) {
                    modalCriarEvento.abrir(agendaCalendario.agendaSelecionadaAtualId, diaSelecionado.startStr);
                }
            }
        });
        agendaCalendario.calendarioInstance.render();
        if (eventosDados.usuarioAtualProprietario === true)
            document.querySelector(".fc-novoEventoBtn-button").setAttribute("data-html2canvas-ignore", "");

        document.querySelector(".fc-gerarImagemBtn-button").setAttribute("data-html2canvas-ignore", "");
    },
    gerarImagemCalendario() {
        const captureElement = document.querySelector('[data-canva]');
        html2canvas(captureElement)
            .then(canvas => {
                canvas.style.display = 'none'
                document.body.appendChild(canvas)
                return canvas
            })
            .then(canvas => {
                const image = canvas.toDataURL('image/png')
                const a = document.createElement('a')
                a.setAttribute('download', 'calendario.png')
                a.setAttribute('href', image)
                a.click()
                canvas.remove()
            });
    }, 
    toggleSelecionarDataEmCalendario(dataSelecionavel = true) {
        agendaCalendario.calendarioInstance.setOption('selectable', dataSelecionavel);
    }
}