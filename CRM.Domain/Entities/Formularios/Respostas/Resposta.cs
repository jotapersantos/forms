using System;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;

namespace CRM.Domain.Entities.Formularios.Respostas;

public abstract class Resposta : Entity
{
    public Guid GabaritoId { get; private set; }
    public Gabarito Gabarito { get; private set; }

    public Guid PerguntaId { get; private set; }
    public Pergunta Pergunta { get; private set; }

    public virtual string Tipo { get; private set; }

    protected Resposta(Guid perguntaId, string tipo)
    {
        PerguntaId = perguntaId;
        Tipo = tipo;
    }

    protected Resposta() { }
}
