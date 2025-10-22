using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRM.WebUI.Helpers;

namespace CRM.WebUI.ViewModels.Formularios.Modelos;

public record PerguntaCadastroVM
{
    public Guid Id { get; set; }
    public string Tipo { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(255, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 3)]
    public string Enunciado { get; set; }

    [Display(Name = "Descrição")]
    public string Descricao { get; set; }

    [Display(Name = "Quantidade mínima de caracteres")]
    [RangeQuantidadeCaracteres(nameof(Tipo), nameof(QuantidadeMinimaCaracteres))]
    [QuantidadeMinimaCaracteresMaiorQueQuantidadeMaxima(nameof(Tipo), nameof(QuantidadeMaximaCaracteres))]
    public int? QuantidadeMinimaCaracteres { get; set; }

    [Display(Name = "Quantidade máxima de caracteres")]
    [RangeQuantidadeCaracteres(nameof(Tipo), nameof(QuantidadeMaximaCaracteres))]
    public int? QuantidadeMaximaCaracteres { get; set; }
    public int? TipoTexto { get; set; }
    public bool Obrigatorio { get; set; }
    public int Ordem { get; set; }
    public Guid SecaoId { get; set; }
    public IEnumerable<AlternativaCadastroVM> Alternativas { get; set; } = new List<AlternativaCadastroVM>();
}
