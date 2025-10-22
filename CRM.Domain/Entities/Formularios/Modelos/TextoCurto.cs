using System;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Enums;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios.Modelos;

public sealed class TextoCurto : PerguntaDiscursiva
{
    public ETipoTextoCurto? TipoTexto { get; private set; }

    private TextoCurto(Guid id,
                    string enunciado,
                    string descricao,
                    bool obrigatorio,
                    int ordem,
                    int? tipoTexto,
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
        TipoTexto = tipoTexto.HasValue ? ETipoTextoCurto.FromValue(tipoTexto.Value) : null;
    }


    public static TextoCurto Build(Guid id, 
                                  string enunciado,
                                  string descricao,
                                  bool obrigatorio,
                                  int ordem,
                                  int? tipoTexto,
                                  int? quantidadeMinimaCaracteres = null,
                                  int? quantidadeMaximaCaracteres = null)
        => new(id, 
               enunciado,
               descricao,
               obrigatorio,
               ordem,
               tipoTexto,
               quantidadeMinimaCaracteres,
               quantidadeMaximaCaracteres);

    public override void Update(Pergunta pergunta)
    {
        base.Update(pergunta);

        if(pergunta is TextoCurto textoCurto)
        {
            TipoTexto = textoCurto.TipoTexto;
        }

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
            DomainValidation.IsGreaterThan(QuantidadeMaximaCaracteres.Value, 250, "Quantidade máxima de caracteres");
        }

        if (QuantidadeMinimaCaracteres.HasValue && QuantidadeMaximaCaracteres.HasValue)
        {
            DomainValidation.IsValidRange(QuantidadeMinimaCaracteres.Value, QuantidadeMaximaCaracteres.Value, "Quantidade mínima de caracteres", "Quantidade máxima de caracteres");
        }
    }

    private TextoCurto() : base() { }
}
