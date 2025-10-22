using System;
using System.Collections.Generic;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Entities.Formularios.Respostas;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios.Modelos;

public class Alternativa : Entity
{
    public string Texto { get; private set; }
    public int Ordem { get; private set; }
    public Guid PerguntaId { get; private set; }
    public Pergunta Pergunta { get; private set; }

    public IEnumerable<RespostaObjetiva> PerguntasRespondidas { get; private set; }

    private Alternativa(Guid id, string texto, int ordem)
    {
        Id = id;
        Texto = texto;
        Ordem = ordem;

        Validate();
    }

    public static Alternativa Build(Guid id,
                                    string texto,
                                    int ordem)
        => new(id,
               texto,
               ordem);

    public void Update(string texto, int ordem)
    {
        Texto = texto;
        Ordem = ordem;
        
        Validate();
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Texto, nameof(Texto));
        DomainValidation.MinLength(Texto, 3, nameof(Texto));
        DomainValidation.MaxLength(Texto, 250, nameof(Texto));
    }
}
