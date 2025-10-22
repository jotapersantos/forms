using System;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios.Modelos;

public sealed class TextoLongo : PerguntaDiscursiva
{
    private TextoLongo(Guid id,
                      string enunciado,
                      string descricao,
                      bool obrigatorio,
                      int ordem,
                      int? quantidadeMinimaCaracteres = null,
                      int? quantidadeMaximaCaracteres = null)
                    : base(id,
                           enunciado,
                           descricao,
                           obrigatorio,
                           ordem,
                           quantidadeMinimaCaracteres,
                           quantidadeMaximaCaracteres)
    {
    }

    public static TextoLongo Build(Guid id,
                      string enunciado,
                      string descricao,
                      bool obrigatorio,
                      int ordem,
                      int? quantidadeMinimaCaracteres = null,
                      int? quantidadeMaximaCaracteres = null)

        => new(id,
               enunciado,
               descricao,
               obrigatorio,
               ordem,
               quantidadeMinimaCaracteres,
               quantidadeMaximaCaracteres);

    public override void Update(Pergunta pergunta)
    {
        base.Update(pergunta);

        Validate();
    }

    protected override void Validate()
    {
        if (QuantidadeMinimaCaracteres.HasValue)
        {
            DomainValidation.IsLessThan(QuantidadeMinimaCaracteres.Value, 1, "Quantidade mínima de caracteres");
        }

        if (QuantidadeMaximaCaracteres.HasValue)
        {
            DomainValidation.IsGreaterThan(QuantidadeMaximaCaracteres.Value, 5000, "Quantidade máxima de caracteres");
        }

        if (QuantidadeMinimaCaracteres.HasValue && QuantidadeMaximaCaracteres.HasValue)
        {
            DomainValidation.IsValidRange(QuantidadeMinimaCaracteres.Value, QuantidadeMaximaCaracteres.Value, "Quantidade mínima de caracteres", "Quantidade máxima de caracteres");
        }
    }
}
