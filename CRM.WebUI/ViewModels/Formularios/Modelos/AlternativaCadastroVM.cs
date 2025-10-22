using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.WebUI.ViewModels.Formularios.Modelos;

public record AlternativaCadastroVM
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(255, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 3)]
    public string Texto { get; set; }
    public int Ordem { get; set; }
    public Guid PerguntaId { get; set; }
}
