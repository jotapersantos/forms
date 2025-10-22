using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.WebUI.ViewModels.Formularios.Modelos;

public record ModeloCadastroVM
{
    public Guid Id { get; set; }

    [Display(Name = "Título")]
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(250, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 3)]
    public string Titulo { get; set; }

    [Display(Name = "Ativo?")]
    public bool Ativo { get; set; }
    public IEnumerable<SecaoCadastroVM> Secoes { get; set; } = new List<SecaoCadastroVM>();
}
