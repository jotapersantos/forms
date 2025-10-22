using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;

namespace CRM.Application.Services.Formularios.Patterns.Interfaces;

public interface IPerguntaChainOR
{
    Pergunta GerarPergunta(PerguntaDTO perguntaDto);
    void SetProximo(IPerguntaChainOR proximo);
}
