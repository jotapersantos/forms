using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Application.DTOs.Formularios.Respostas;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Entities.Formularios.Respostas;

namespace CRM.Application.Services.Formularios.Respostas;
internal static class RespostaFactory
{
    public static IEnumerable<Resposta> GerarRespostas(IEnumerable<Pergunta> perguntas, IEnumerable<RespostaDTO> respostasDto)
    {
        var respostas = new List<Resposta>();

        foreach (Pergunta pergunta in perguntas)
        {
            RespostaDTO respostaDaPergunta = respostasDto.FirstOrDefault(resposta => resposta.PerguntaId == pergunta.Id);

            if (respostaDaPergunta != null)
            {
                Resposta resposta;

                switch (pergunta)
                {
                    case TextoCurto:
                    case TextoLongo:
                        resposta = RespostaDiscursiva.Build(pergunta.Id, respostaDaPergunta.Texto);
                        break;
                    case MultiplaEscolha:
                    case CaixaSelecao:
                    case ListaSuspensa:
                        IEnumerable<Alternativa> alternativasSelecionadas = ObterAlternativasSelecionadasDaPergunta(pergunta, respostaDaPergunta.AlternativasSelecionadasIds);
                        resposta = RespostaObjetiva.Build(pergunta.Id, alternativasSelecionadas);
                        break;
                    default:
                        throw new NotImplementedException("Tipo de pergunta não implementado");
                }

                respostas.Add(resposta);
            }
        }

        return respostas;
    }

    private static IEnumerable<Alternativa> ObterAlternativasSelecionadasDaPergunta(Pergunta pergunta, IEnumerable<Guid> alternativasSelecionadas)
    {
        return ((PerguntaObjetiva)pergunta).Alternativas
                                           .Where(alternativa => alternativasSelecionadas.Contains(alternativa.Id))
                                           .ToList();
    }
}
