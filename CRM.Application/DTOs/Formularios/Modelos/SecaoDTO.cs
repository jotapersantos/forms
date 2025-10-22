using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Application.DTOs.Formularios.Modelos;

public class SecaoDTO
{
    public Guid Id { get; set; }

    [Display(Name = "Título da seção")]
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(250, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 3)]
    public string Titulo { get; set; }
    public int Ordem { get; set; }
    public Guid ModeloId { get; set; }
    public ModeloDTO Modelo { get; set; }
    public IEnumerable<PerguntaDTO> Perguntas { get; set; }
}
