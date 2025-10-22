using CRM.Domain.Entities.Formularios.Modelos;
using System;
using System.Collections.Generic;

namespace CRM.Domain.Entities.Formularios.Respostas;
public sealed class RespostaObjetiva : Resposta
{
    private readonly List<Alternativa> _alternativasSelecionadas = [];
    public IReadOnlyCollection<Alternativa> AlternativasSelecionadas => _alternativasSelecionadas.AsReadOnly();

    public static RespostaObjetiva Build(Guid perguntaId, IEnumerable<Alternativa> alternativasSelecionadas)
        => new(perguntaId, alternativasSelecionadas);

    private RespostaObjetiva(Guid perguntaId, IEnumerable<Alternativa> alternativasSelecionadas)
        : base(perguntaId, "Objetiva")
    {
        Id = Guid.Empty;
        _alternativasSelecionadas.AddRange(alternativasSelecionadas);
    }

    private RespostaObjetiva() : base() { }
}
