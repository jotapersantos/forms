using System;
using System.Collections.Generic;
using CRM.Application.DTOs.Formularios.Modelos;

namespace CRM.Application.DTOs.Formularios.Respostas;

public class RespostaDTO
{
    public Guid Id { get; set; }

    public Guid PerguntaId { get; set; }
    public PerguntaDTO Pergunta { get; set; }

    public List<Guid> AlternativasSelecionadasIds { get; set; }//Resposta para pergunta de múltipla escolha

    public IEnumerable<AlternativaDTO> AlternativasSelecionadas { get; set; }
    public string Texto { get; set; } //Resposta para pergunta de texto
}
