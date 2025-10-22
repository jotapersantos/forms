using System;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Entities.Formularios.Respostas;

namespace CRM.Domain.Entities.Formularios.Modelos;

public sealed class MultiplaEscolha : PerguntaObjetiva
{
    private MultiplaEscolha(Guid id,
                            string enunciado,
                            string descricao,
                            bool obrigatorio,
                            int ordem)
                        : base(id,
                                enunciado,
                                descricao,
                                obrigatorio,
                                ordem)
    {
    }

    public static MultiplaEscolha Build(Guid id, 
                                        string enunciado,
                                        string descricao,
                                        bool obrigatorio,
                                        int ordem)
        => new(id,
               enunciado,
               descricao,
               obrigatorio,
               ordem);

    public override void Update(Pergunta pergunta)
    {
        base.Update(pergunta);
    }

    public override bool IsRespostaValida(Resposta resposta)
    {
        if (!(resposta is RespostaObjetiva respostaObjetiva))
        {
            throw new ArgumentException("O tipo da resposta é invalido.");
        }

        int quantidadeDeAlternativasSelecionadas = respostaObjetiva.AlternativasSelecionadas.Count;

        if (Obrigatorio)
        {
            if (quantidadeDeAlternativasSelecionadas == 1)
            {
                return true;
            }

            throw new InvalidOperationException("Quantidade de alternativas selecionadas inválida.");
        }
        else
        {
            if (quantidadeDeAlternativasSelecionadas == 0 ||
                quantidadeDeAlternativasSelecionadas == 1)
            {
                return true;
            }
        }

        throw new InvalidOperationException("Resposta inválida.");
    }

    private MultiplaEscolha() : base(){ }
}
