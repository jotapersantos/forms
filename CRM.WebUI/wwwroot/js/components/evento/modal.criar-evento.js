const modalCriarEvento = {
    urlCarregarModalCriarEvento: null,
    criarEventoModalElemento: null,
    criarEventoModalInstance: null,
    inicializar(urlCarregarModalCriarEvento) {
        this.urlCarregarModalCriarEvento = urlCarregarModalCriarEvento;
        this.criarEventoModalElemento = document.querySelector("#criar_evento_modal");
    },
    abrir(agendaId, dataSelecionada = null) {
        let url = `${this.urlCarregarModalCriarEvento}`.replace("00000000-0000-0000-0000-000000000000", agendaId);

        if (dataSelecionada !== null)
            url += `?dataEvento=${encodeURIComponent(dataSelecionada)}`;

        fetch(url, {
            method: 'GET',
        }).then(response => response.text())
            .then(response => {
                modalCriarEvento.criarEventoModalInstance = new bootstrap.Modal(modalCriarEvento.criarEventoModalElemento);
                modalCriarEvento.criarEventoModalElemento.querySelector(".modal-content").innerHTML = response;
                modalCriarEvento.criarEventoModalInstance.show();
                modalCriarEvento.configurarDateRangePicker();
                modalCriarEvento.configurarFlatpickr();
                modalCriarEvento.configurarSpectrum();
                modalCriarEvento.configurarDateRangePickerComDataMinima(moment().toDate(), "Recorrencia_DataLimite");
                modalCriarEvento.configurarValidacaoCamposDatas();
                modalCriarEvento.configurarEnvioFormulario();
                modalCriarEvento.configurarValidacaoFormulario();
                modalCriarEvento.configurarVerificacaoCampoDiaInteiro();
                modalCriarEvento.configurarAlteracaoDataMinimaParaDataTermino();
                modalCriarEvento.configurarVerificacaoCampoRecorrente();
                modalCriarEvento.configurarAlteracaoPluralTipoRecorrencias();
                modalCriarEvento.configurarAcoesAdicionaisAposFechar();
            });
    },
    configurarEnvioFormulario() {
        document.querySelector("#criar_evento_form").addEventListener("submit", function (event) {
            modalCriarEvento.criarEvento(event);
        });
    },
    configurarVerificacaoCampoDiaInteiro() {
        const campoDiaInteiro = document.querySelector("#DiaInteiro");
        campoDiaInteiro.value = false;
        campoDiaInteiro.addEventListener("change", function (event) {
            modalCriarEvento.toggleDiaInteiro(event.target);
        });
    },
    configurarVerificacaoCampoRecorrente() {
        const campoRecorrente = document.querySelector("#Recorrente");
        campoRecorrente.value = false;
        modalCriarEvento.toggleInformacoesRecorrencia(campoRecorrente);
        campoRecorrente.addEventListener("change", function (event) {
            modalCriarEvento.toggleInformacoesRecorrencia(event.target);
        })
    },
    configurarDateRangePicker() {
        moment.locale('pt-br');
        $(".daterangepicker-custom").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoApply: true,
            locale: {
                format: "DD/MM/YYYY",
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
            }
        });
    },
    configurarDateRangePickerComDataMinima(dataMinima, campoId = null) {
        moment.locale('pt-br');
        const identificador = campoId != null ? `#${campoId}` : ".daterangepicker-custom";
        $(identificador).daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoApply: true,
            minDate: moment(dataMinima).format("DD/MM/YYYY"),
            locale: {
                format: "DD/MM/YYYY",
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
            },
        });
    },
    configurarFlatpickr() {
        $("#HoraInicio").flatpickr({
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i",
            minuteIncrement: 1,
            time_24hr: true,
            altInput: false,
            onChange: function (dateObj, dateStr) {
                var horarioSplit = dateStr.split(':');
                var horaMoment = moment().hours(Number(horarioSplit[0])).minutes(Number(horarioSplit[1]) + 30).toDate();

                const horaTerminoInstance = $("#HoraTermino")[0]._flatpickr;
                horaTerminoInstance.setDate(horaMoment);
            }
        });

        $("#HoraTermino").flatpickr({
            enableTime: true,
            noCalendar: true,
            minuteIncrement: 1,
            dateFormat: "H:i",
            time_24hr: true,
            altInput: false,
            onChange: function (dateObj, dateStr) {
                $("#DataFim").valid();
            }
        });
    },
    configurarSpectrum() {
        $("#CorEvento").spectrum({
            showPaletteOnly: true,
            preferredFormat: 'hex',
            showPalette: true,
            color: '#3788d8',
            hideAfterPaletteSelect: true,
            palette: [
                ['rgb(28, 28, 28)', 'rgb(125, 125, 125)', '#3788d8',
                    'rgb(255, 128, 0);', 'hsv 100 70 50'],
                ['red', '#fccf03', 'green', 'blue', 'violet']
            ],
            change: function (color) {
                color.toHexString();
            }
        });
    },
    configurarAlteracaoDataMinimaParaDataTermino() {
        const dataInicioInput = document.querySelector("#DataInicio");
        const dataFimInput = document.querySelector("#DataFim");

        $(dataInicioInput).on("apply.daterangepicker", function () {
            dataInicioInput.dispatchEvent(new Event('change'));
        });

        dataInicioInput.addEventListener("change", function () {
            $(dataFimInput).daterangepicker({
                startDate: dataInicioInput.value,
                minDate: dataInicioInput.value,
                singleDatePicker: true,
                autoApply: true,
            });
        });
    },
    
    alterarValueCheckbox(checkbox) {
        checkbox.value = checkbox.checked;
    },
    toggleDiaInteiro(campoDiaInteiro) {
        modalCriarEvento.alterarValueCheckbox(campoDiaInteiro);
        const horaInicioElemento = document.querySelector("#HoraInicio").closest("div");
        const horaTerminoElemento = document.querySelector("#HoraTermino").closest("div");
        if (campoDiaInteiro.checked === true) {
            horaInicioElemento.classList.add("d-none");
            horaTerminoElemento.classList.add("d-none");
        }
        else {
            horaInicioElemento.classList.remove("d-none");
            horaTerminoElemento.classList.remove("d-none");
        }
    },
    toggleInformacoesRecorrencia(campoRecorrente) {
        modalCriarEvento.alterarValueCheckbox(campoRecorrente);
        const informacoesRecorrenciaContainer = document.querySelector("#informacoes-recorrencia-container");
        if (campoRecorrente.checked) {
            informacoesRecorrenciaContainer.classList.remove("d-none");
        }
        else {
            informacoesRecorrenciaContainer.classList.add("d-none");
        }
    },
    criarEvento(event) {
        event.preventDefault();
        const criarEventoFormElemento = document.querySelector("#criar_evento_form");

        if ($(criarEventoFormElemento).valid()) {
            const criarEventoFormData = new FormData(criarEventoFormElemento);

            if (criarEventoFormData.get("DiaInteiro") === "true") {
                criarEventoFormData.set("DataInicio", modalCriarEvento.gerarDataComHorarioEmString(criarEventoFormData.get("DataInicio"), "00:00"));
                criarEventoFormData.set("DataFim", modalCriarEvento.gerarDataComHorarioEmString(criarEventoFormData.get("DataFim"), "23:59"));
            }
            else {
                criarEventoFormData.set("DataInicio", modalCriarEvento.gerarDataComHorarioEmString(criarEventoFormData.get("DataInicio"), criarEventoFormData.get("HoraInicio")));
                criarEventoFormData.set("DataFim", modalCriarEvento.gerarDataComHorarioEmString(criarEventoFormData.get("DataFim"), criarEventoFormData.get("HoraTermino")));
            }
           
            fetch(criarEventoFormElemento.getAttribute("action"), {
                method: criarEventoFormElemento.getAttribute("method"),
                body: criarEventoFormData
            }).then(response => response.json())
                .then(response => {
                    if (response.isSuccess === true) {
                        modalCriarEvento.criarEventoModalInstance.hide();
                        finalMessage = "";
                        response.messages.forEach((message) => {
                            finalMessage += message;
                        })
                        toastr.success(finalMessage);
                        setTimeout(function () { window.location.href = response.urlRedirect }, 1500);
                    }
                });
        }
    },
    configurarValidacaoCamposDatas() {
        $.validator.addMethod('datafiminvalida', function (value, element, params) {
            var dataInicio = modalCriarEvento.gerarDataComHorario($(`#${params.datainicio}`).val(), $("#HoraInicio").val());
            var dataFim = modalCriarEvento.gerarDataComHorario($("#DataFim").val(), $("#HoraTermino").val());
            if (dataFim < dataInicio) return false;
            return true;
        });

        $.validator.unobtrusive.adapters.add('datafiminvalida', ['datainicio'], function (options) {
            options.rules['datafiminvalida'] = {
                datainicio: options.params['datainicio']
            }
            options.messages['datafiminvalida'] = options.message;
        });
    },
    configurarValidacaoFormulario() {
        const criarEventoFormElemento = document.querySelector("#criar_evento_form");
        $.validator.unobtrusive.parse(criarEventoFormElemento);
    },
    configurarAlteracaoPluralTipoRecorrencias() {
        $("#Recorrencia_Intervalo").change(function (event) {
            var intervalos = parseInt(event.target.value);
            var opcoesSingular = ["dia", "semana", "mês", "ano"];
            var opcoesPlural = ["dias", "semanas", "meses", "anos"];
            var singular = intervalos === 1;

            $("#Recorrencia_TipoRecorrencia option").each(function (index, item) {
                if (!singular && intervalos > 0) {
                    $(item).html(opcoesPlural[index]);
                }
                else {
                    $(item).html(opcoesSingular[index]);
                }
            });
        });
    },
    configurarAcoesAdicionaisAposFechar() {
        modalCriarEvento.criarEventoModalElemento.addEventListener("hide.bs.modal", function () {
            agendaCalendario.toggleSelecionarDataEmCalendario();
        });
    },
    gerarDataComHorario(data, hora) {
        return moment(data + " " + hora, "DD/MM/YYYY HH:mm").toDate();
    },
    gerarDataComHorarioEmString(data, hora) {
        const dataMoment = moment(data, "DD/MM/YYYY");
        const horaMoment = moment(hora, "HH:mm");

        const dataHoraMoment = dataMoment.set({
            hour: horaMoment.get('hour'),
            minute: horaMoment.get('minute')
        });

        return dataHoraMoment.format("DD/MM/YYYY HH:mm");
    }
}
