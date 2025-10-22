using System;
using System.Collections.Generic;
using CRM.Domain.Entities.Formularios.Respostas;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios.Modelos.Abstractions;

public abstract class Pergunta : Entity
{
    public string Enunciado { get; private set; }
    public string Descricao { get; private set; }
    public bool Obrigatorio { get; private set; }
    public int Ordem { get; private set; }
    public Guid SecaoId { get; private set; }
    public Secao Secao { get; private set; }
    public virtual string Tipo { get; private set; }

    public List<Resposta> Respostas { get; private set; }

    protected Pergunta(Guid id, string enunciado, string descricao, bool obrigatorio, int ordem)
    {
        Id = id;
        Enunciado = enunciado;
        Descricao = descricao;
        Obrigatorio = obrigatorio;
        Ordem = ordem;

        Validate();
    }

    public virtual void Update(Pergunta pergunta)
    {
        Enunciado = pergunta.Enunciado;
        Descricao = pergunta.Descricao;
        Obrigatorio = pergunta.Obrigatorio;
        Ordem = pergunta.Ordem;
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Enunciado, nameof(Enunciado));
        DomainValidation.MinLength(Enunciado, 3, nameof(Enunciado));
        DomainValidation.MaxLength(Enunciado, 250, nameof(Enunciado));

        DomainValidation.MaxLength(Descricao, 1000, "Descrição");
    }

    public abstract bool IsRespostaValida(Resposta resposta);

    protected Pergunta() {}
}
