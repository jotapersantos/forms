using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Application.Services.Formularios.Patterns.Interfaces;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;

namespace CRM.Application.Services.FormularioSecaoItens;

public class TextoLongoChainOR : IPerguntaChainOR
{
    public IPerguntaChainOR Proximo { get; set; }

    public Pergunta GerarPergunta(PerguntaDTO perguntaDto)
    {
        if (perguntaDto.Tipo == "TextoLongo")
        {
            return TextoLongo.Build(
                    perguntaDto.Id,
                    perguntaDto.Enunciado,
                    perguntaDto.Descricao,
                    perguntaDto.Obrigatorio,
                    perguntaDto.Ordem,
                    perguntaDto.QuantidadeMinimaCaracteres,
                    perguntaDto.QuantidadeMaximaCaracteres
                );
        }

        return Proximo.GerarPergunta(perguntaDto);
    }

    public void SetProximo(IPerguntaChainOR proximo)
    {
        Proximo = proximo;
    }
}
