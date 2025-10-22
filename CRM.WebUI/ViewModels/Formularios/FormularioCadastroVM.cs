using System;
using System.ComponentModel.DataAnnotations;
using CRM.WebUI.Helpers;

namespace CRM.WebUI.ViewModels.Formularios;

public class FormularioCadastroVM
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(250, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 3)]
    public string Nome { get; set; }

    [Display(Name = "Mensagem de Confirmação")]
    [MaxLength(250, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string MensagemConfirmacao { get; set; }

    [Display(Name = "Data de Ínicio")]
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public DateOnly DataInicio { get; set; }

    [Display(Name = "Hora de Ínicio")]
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public TimeOnly HoraInicio { get; set; }

    [Display(Name = "Data de Término")]
    [DataTerminoMaiorQueDataInicio("DataInicio", "HoraInicio", "HoraTermino", ErrorMessage = "A data de término deve ser maior que a data de início")]
    public DateOnly? DataTermino { get; set; }

    [Display(Name = "Hora de Término")]
    public TimeOnly? HoraTermino { get; set; }

    [Display(Name = "Modelo de Formulário")]
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid ModeloId { get; set; }
    public string ModeloTitulo { get; set; }
}
