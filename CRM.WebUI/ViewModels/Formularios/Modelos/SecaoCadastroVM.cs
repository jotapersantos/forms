using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.WebUI.ViewModels.Formularios.Modelos;
public record SecaoCadastroVM
{
    public Guid Id { get; set; }

    [Display(Name = "Título da seção")]
    [MaxLength(250, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Titulo { get; set; }
    public int Ordem { get; set; }
    public Guid ModeloId { get; set; }
    public IEnumerable<PerguntaCadastroVM> Perguntas { get; set; } = new List<PerguntaCadastroVM>();
}
