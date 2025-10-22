const formularioCadastro = {
    urlObterFormularioModelos: null,
    inicializar(urlObterFormularioModelos = null) {
        this.urlObterFormularioModelos = urlObterFormularioModelos;
        this.configureSelectFormularioModelos();
        this.configureDateRangePicker();
        this.configureFlatPickr();
        this.configureMaxlength();
        this.configurarAutosize();
        this.configureCustomValidationsInForm();
        this.configureSubmit();
    },
    configureSelectFormularioModelos() {
        if (this.urlObterFormularioModelos) {
            const selectFormularioModelos = document.querySelector('[name="ModeloId"]');

            $(selectFormularioModelos).select2({
                language: "pt-BR",
                placeholder: "Selecione um item",
                minimumInputLength: 3,
                allowClear: true,
                ajax: {
                    url: this.urlObterFormularioModelos,
                    dataType: 'json',
                    delay: 250,
                    data: (params) => {
                        return {
                            searchString: params.term,
                            quantity: 10,
                        };
                    },
                    processResults: (data) => {
                        var formattedData = $.map(data, function (value, key) {
                            return {
                                id: key,    // GUID
                                text: value // ESPECIALIDADE
                            };
                        });

                        return {
                            results: formattedData
                        };
                    },
                    cache: true
                }
            });
        }
    },
    configureMaxlength() {
        $('.maxlength').maxlength({
            threshold: 20,
            warningClass: "badge badge-primary",
            limitReachedClass: "badge badge-success"
        });
    },
    configurarAutosize() {
        document.querySelectorAll(".autosize-custom").forEach(function (textarea) {
            autosize(textarea);
        });
    },
    configureDateRangePicker() {
        moment.locale('pt-br');
        $(".daterangepicker-custom").daterangepicker({
            autoUpdateInput: false,
            singleDatePicker: true,
            showDropdowns: true,
            autoApply: true,
            minDate: moment().format("DD/MM/YYYY"),
            locale: {
                format: "DD/MM/YYYY",
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
            }
        }).on('apply.daterangepicker', (ev, picker) => {
            $(ev.target).val(picker.startDate.format("DD/MM/YYYY"));
            $(`[name="DataTermino"]`).valid();
        });

        Inputmask({
            mask: "99/99/9999",
            alias: "date",
            placeholder: "__/__/____",
            insertMode: false
        }).mask(".daterangepicker-custom");
    },
    configureFlatPickr() {
        $(".timepicker").flatpickr({
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i",
            minuteIncrement: 1,
            time_24hr: true,
            altInput: false,
            allowInput: true,
            onChange: (dateObj, dateStr) => {
                $('[name="DataTermino"]').valid();
            }
        });

        Inputmask({
            mask: "99:99",
            alias: "time",
            placeholder: "__:__",
            insertMode: false
        }).mask(".timepicker");
    },
    configureValidationCampoDataTermino() {
        $.validator.addMethod('dataterminovalida', (value, element, params) => {
            var dataInicio = this.combineDateAndTime($(`#${params.datainicio}`).val(), $(`#${params.horainicio}`).val());
            var dataFim = this.combineDateAndTime(value, $(`#${params.horatermino}`).val());

            if (dataFim === null) return true;

            return dataInicio < dataFim;
        });

        $.validator.unobtrusive.adapters.add('dataterminovalida', ['datainicio', 'horainicio', 'horatermino'], function (options) {
            options.rules['dataterminovalida'] = {
                datainicio: options.params['datainicio'],
                horainicio: options.params['horainicio'],
                horatermino: options.params['horatermino']
            }
            options.messages['dataterminovalida'] = options.message;
        });
    },
    configureCustomValidationsInForm() {
        this.configureValidationCampoDataTermino();
        const formularioCadastroForm = document.querySelector('[data-formulario-cadastro="formulario"]');
        $.validator.unobtrusive.parse(formularioCadastroForm);
    },
    configureSubmit() {
        document.querySelector('[data-formulario-cadastro="formulario"]').addEventListener("submit", (event) => {
            event.preventDefault();
            if ($('[data-formulario-cadastro="formulario"]').valid()) {
                this.salvar();
            }
        });
    },
    combineDateAndTime(dateString, timeString) {
        if (dateString.length === 0 || timeString.length === 0) {
            return null;
        }
        return moment(`${dateString} ${timeString}`, "DD/MM/YYYY HH:mm").toDate();
    },
    salvar() {
        const formularioCadastroFormElemento = document.querySelector('[data-formulario-cadastro="formulario"]');

        const formularioCadastroForm = new FormData(formularioCadastroFormElemento);

        fetch(formularioCadastroFormElemento.getAttribute("action"), {
            method: formularioCadastroFormElemento.getAttribute("method"),
            body: formularioCadastroForm
        }).then(response => response.json())
            .then(response => {
                if (response.isSuccess === true) {
                    const message = response.messages.join("\n");
                    toastr.success(message, {timeOut: 1500 });
                    setTimeout(function () { window.location.href = response.urlRedirect }, 1500);
                }
                else {
                    const error = new Error(response.messages);
                    throw error;
                }
            }).catch(error => {
                toastr.error(error.message, { timeOut: 3000 });
            });
    }
}