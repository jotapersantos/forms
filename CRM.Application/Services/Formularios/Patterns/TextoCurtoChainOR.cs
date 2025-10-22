using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Application.Services.Formularios.Patterns.Interfaces;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;

namespace CRM.Application.Services.Formularios.Patterns;

public class TextoCurtoChainOR : IPerguntaChainOR
{
    public IPerguntaChainOR Proximo { get; set; }

    public Pergunta GerarPergunta(PerguntaDTO perguntaDto)
    {
        if (perguntaDto.Tipo == "TextoCurto")
        {
            return TextoCurto.Build(
                    perguntaDto.Id,
                    perguntaDto.Enunciado,
                    perguntaDto.Descricao,
                    perguntaDto.Obrigatorio,
                    perguntaDto.Ordem,
                    perguntaDto.TipoTexto,
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
