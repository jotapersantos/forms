using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Application.DTOs.Formularios.Modelos;

public class PerguntaDTO
{
    public Guid Id { get; set; }
    public string Tipo { get; set; }
    public string Enunciado { get; set; }
    public string Descricao { get; set; }
    public int? QuantidadeMinimaCaracteres { get; set; }
    public int? QuantidadeMaximaCaracteres { get; set; }
    public int? TipoTexto { get; set; }
    public bool Obrigatorio { get; set; }
    public int Ordem { get; set; }
    public Guid SecaoId { get; set; }
    public IEnumerable<AlternativaDTO> Alternativas { get; set; }
}
