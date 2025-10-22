using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;

namespace CRM.Domain.Entities.Formularios.Respostas;

public abstract class Gabarito : Entity
{
    public DateTime? RespondidoEm { get; protected set; }
    public virtual string Tipo { get; private set; }

    private readonly List<Resposta> _respostas = [];
    public IReadOnlyCollection<Resposta> Respostas => _respostas.AsReadOnly();

    protected Gabarito()
    {
        Id = Guid.Empty;
    }

    protected void Responder(IEnumerable<Pergunta> perguntas, IEnumerable<Resposta> respostas)
    {
        foreach (Pergunta pergunta in perguntas)
        {
            Resposta resposta = respostas.FirstOrDefault(resposta => resposta.PerguntaId == pergunta.Id);

            if (resposta != null)
            {
                pergunta.IsRespostaValida(resposta);

                _respostas.Add(resposta);
            }
        }

        RespondidoEm = DateTime.UtcNow;
    }
}
