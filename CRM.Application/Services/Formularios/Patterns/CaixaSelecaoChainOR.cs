using System.Collections.Generic;
using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Application.Services.Formularios.Patterns.Interfaces;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;

namespace CRM.Application.Services.FormularioSecaoItens;

public class CaixaSelecaoChainOR : IPerguntaChainOR
{
    public IPerguntaChainOR Proximo { get; set; }

    public Pergunta GerarPergunta(PerguntaDTO perguntaDto)
    {
        if (perguntaDto.Tipo == "CaixaSelecao")
        {
            var pergunta = CaixaSelecao.Build(
                perguntaDto.Id,
                perguntaDto.Enunciado,
                perguntaDto.Descricao,
                perguntaDto.Obrigatorio,
                perguntaDto.Ordem
            );

            var alternativas = new List<Alternativa>();

            foreach (AlternativaDTO alternativaDto in perguntaDto.Alternativas)
            {
                var alternativa = Alternativa.Build(alternativaDto.Id,
                                                    alternativaDto.Texto,
                                                    alternativaDto.Ordem);

                alternativas.Add(alternativa);
            }

            pergunta.AdicionarAlternativas(alternativas);

            return pergunta;
        }

        return Proximo.GerarPergunta(perguntaDto);
    }

    public void SetProximo(IPerguntaChainOR proximo)
    {
        Proximo = proximo;
    }
}
