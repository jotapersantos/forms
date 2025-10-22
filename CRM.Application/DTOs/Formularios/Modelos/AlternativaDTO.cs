using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Application.DTOs.Formularios.Modelos;

public class AlternativaDTO
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(255, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 3)]
    public string Texto { get; set; }
    public int Ordem { get; set; }
    public Guid PerguntaId { get; set; }
    public PerguntaDTO Pergunta { get; set; }
}
