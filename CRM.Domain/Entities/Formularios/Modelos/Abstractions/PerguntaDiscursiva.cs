using System;
using CRM.Domain.Entities.Formularios.Respostas;

namespace CRM.Domain.Entities.Formularios.Modelos.Abstractions;

public abstract class PerguntaDiscursiva : Pergunta
{
    public int? QuantidadeMinimaCaracteres { get; private set; }
    public int? QuantidadeMaximaCaracteres { get; private set; }

    public PerguntaDiscursiva(Guid id,
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
                                   ordem)
    {
        QuantidadeMinimaCaracteres = quantidadeMinimaCaracteres;
        QuantidadeMaximaCaracteres = quantidadeMaximaCaracteres;

        Validate();
    }

    public override void Update(Pergunta pergunta)
    {
        base.Update(pergunta);
        
        if(pergunta is PerguntaDiscursiva perguntaDiscursiva)
        {
            QuantidadeMinimaCaracteres = perguntaDiscursiva.QuantidadeMinimaCaracteres;
            QuantidadeMaximaCaracteres = perguntaDiscursiva.QuantidadeMaximaCaracteres;
        }
    }

    public override bool IsRespostaValida(Resposta resposta)
    {
        if (resposta is not RespostaDiscursiva respostaDiscursiva)
        {
            throw new ArgumentException("O tipo da resposta é inválido.");
        }

        if (QuantidadeMinimaCaracteres.HasValue &&
            respostaDiscursiva.Texto.Length < QuantidadeMinimaCaracteres)
        {
            throw new InvalidOperationException($"A resposta deve ter no mínimo {QuantidadeMinimaCaracteres} caracteres.");
        }

        if (QuantidadeMaximaCaracteres.HasValue &&
            respostaDiscursiva.Texto.Length > QuantidadeMaximaCaracteres)
        {
            throw new InvalidOperationException($"A resposta deve ter no máximo {QuantidadeMaximaCaracteres} caracteres.");
        }

        if (Obrigatorio && string.IsNullOrEmpty(respostaDiscursiva.Texto.Trim()))
        {
            throw new InvalidOperationException("Resposta inválida.");
        }

        return true;
    }

    protected abstract void Validate();

    protected PerguntaDiscursiva() : base() { }
}
