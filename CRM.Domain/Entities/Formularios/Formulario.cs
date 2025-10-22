using System;
using System.Collections.Generic;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Entities.Formularios.Respostas;
using CRM.Domain.Entities.Formularios.ValueObjects;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios;

public class Formulario : Entity
{
    public string Nome { get; private set; }
    public string MensagemConfirmacao { get; private set; }

    public Guid ModeloId { get; private set; }
    public Modelo Modelo { get; private set; }

    public DateRange Periodo { get; private set; }

    private readonly List<FormularioGabarito> _gabaritos = [];
    public IReadOnlyCollection<FormularioGabarito> Gabaritos => _gabaritos.AsReadOnly();

    protected Formulario(string nome,
                       string mensagemConfirmacao,
                       DateTime? dataInicio,
                       DateTime? dataTermino,
                       Modelo modelo)
    {
        Nome = nome;
        ModeloId = modelo.Id;
        MensagemConfirmacao = mensagemConfirmacao;
        Periodo = new DateRange(dataInicio, dataTermino);

        Validate();
        
    }

    public static Formulario Build(string nome,
                                   string mensagemConfirmacao,
                                   DateTime? dataInicio,
                                   DateTime? dataTermino,
                                   Modelo modelo)
    {
        DomainValidation.NotNull(modelo, "Modelo");
        DomainValidation.NotNullOrEmpty(modelo.Id, "O campo Id de Modelo");
        DomainValidation.IsTrue(modelo.Ativo, "O campo Ativo de Modelo");

        return new(nome, mensagemConfirmacao, dataInicio, dataTermino, modelo);

    }

    public void Update(string nome,
                       string mensagemConfirmacao,
                       DateTime? dataInicio,
                       DateTime? dataTermino)
    {
        Nome = nome;
        MensagemConfirmacao = mensagemConfirmacao;
        Periodo = new DateRange(dataInicio, dataTermino);

        Validate();
    }

    public virtual void Responder(IEnumerable<Pergunta> perguntas, IEnumerable<Resposta> respostas)
    {
        _gabaritos.Add(FormularioGabarito.Responder(this, perguntas, respostas));
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Nome, "Nome");
        DomainValidation.MinLength(Nome, 3, "Nome");
        DomainValidation.MaxLength(Nome, 250, "Nome");
    }

    protected Formulario(){}
}
