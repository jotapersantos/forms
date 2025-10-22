using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Domain.Entities.Formularios.Respostas;

namespace CRM.Domain.Entities.Formularios.Modelos.Abstractions;

public abstract class PerguntaObjetiva : Pergunta
{
    private readonly List<Alternativa> _alternativas = [];
    public IReadOnlyCollection<Alternativa> Alternativas => _alternativas.AsReadOnly();

    protected PerguntaObjetiva(Guid id,
                            string enunciado,
                            string descricao,
                            bool obrigatorio,
                            int ordem)
                        : base(id,
                               enunciado,
                               descricao,
                               obrigatorio,
                               ordem)
    { }

    public override void Update(Pergunta pergunta)
    {
        var perguntaObjetiva = (PerguntaObjetiva)pergunta;

        base.Update(pergunta);

        AtualizarAlternativas(perguntaObjetiva.Alternativas);
    }

    public void AdicionarAlternativas(IEnumerable<Alternativa> alternativas)
    {
        if(!alternativas.Any())
        {
            throw new InvalidOperationException("A pergunta objetiva deve ter no mínimo uma alternativa.");
        }

        foreach (Alternativa alternativa in alternativas)
        {
            if (_alternativas.Any(p => p.Id == alternativa.Id) && alternativa.Id != Guid.Empty)
            {
                throw new InvalidOperationException("A alternativa já está atribuída à pergunta.");
            }
            _alternativas.Add(alternativa);
        }
    }

    public void AtualizarAlternativas(IEnumerable<Alternativa> alternativasAtualizadas)
    {
        var alternativasAtualizadasIds  = alternativasAtualizadas.Select(a => a.Id).ToList();

        var alternativasParaRemover = _alternativas.Where(a => alternativasAtualizadasIds.Contains(a.Id) == false).ToList();

        RemoverAlternativas(alternativasParaRemover);
       
        foreach(Alternativa alternativaAtualizada in alternativasAtualizadas)
        {
            Alternativa alternativa = _alternativas.SingleOrDefault(a => alternativaAtualizada.Id != Guid.Empty && a.Id == alternativaAtualizada.Id); 

            if(alternativa is null)
            {
                _alternativas.Add(alternativaAtualizada);
            }
            else
            {
                alternativa.Update(alternativaAtualizada.Texto, alternativaAtualizada.Ordem);
            }
        }

    }

    public void RemoverAlternativas(IEnumerable<Alternativa> alternativas)
    {
        var alternativasParaRemover = _alternativas.Where(p => alternativas.Select(ar => ar.Id).Contains(p.Id)).ToList();

        foreach (Alternativa alternativa in alternativasParaRemover)
        {
            if (!_alternativas.Remove(alternativa))
            {
                throw new InvalidOperationException("A alternativa não está atribuída à pergunta.");
            }
        }
    }

    public override bool IsRespostaValida(Resposta resposta)
    {
        if(resposta is not RespostaObjetiva)
        {
            throw new ArgumentException("O tipo da resposta é invalido.");
        }

        return true;
    }

    protected PerguntaObjetiva() : base() { }
}
