using System;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using System.Collections.Generic;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios.Respostas;

public sealed class FormularioGabarito : Gabarito
{
    public Guid FormularioId { get; private set; }
    public Formulario Formulario { get; private set; }

    public static FormularioGabarito Responder(Formulario formulario, IEnumerable<Pergunta> perguntas, IEnumerable<Resposta> respostas)
    {
        var gabarito = new FormularioGabarito(formulario);
        
        gabarito.Responder(perguntas, respostas);

        return gabarito;
    }

    private FormularioGabarito(Formulario formulario)
    {
        if (formulario == null)
        {
            throw new DomainValidationException("É necessário especificar o formulário.");
        }

        FormularioId = formulario.Id;
    }

    private FormularioGabarito() { }
}
