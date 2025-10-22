using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios.Modelos;

public class Secao : Entity
{
    public string Titulo { get; private set; }
    public int Ordem { get; private set; }
    public Guid ModeloId { get; private set; }
    public Modelo Modelo { get; private set; }

    private readonly List<Pergunta> _perguntas = [];
    public IReadOnlyCollection<Pergunta> Perguntas => _perguntas.AsReadOnly();

    private Secao(Guid id, string titulo, int ordem)
    {
        Id = id;
        Titulo = titulo;
        Ordem = ordem;

        Validate();
    }

    public static Secao Build(Guid id, string titulo, int ordem)
        => new Secao(id, titulo, ordem);

    public void Update(string titulo, int ordem)
    {
        Titulo = titulo;
        Ordem = ordem;

        Validate();
    }

    public void AddPerguntas(IEnumerable<Pergunta> perguntas)
    {
        if(perguntas.Count() == 0)
        {
            throw new InvalidOperationException("A seção deve ter no mínimo uma pergunta.");
        }

        foreach (Pergunta pergunta in perguntas)
        {
            if (_perguntas.Any(p => p.Id == pergunta.Id) && pergunta.Id != Guid.Empty)
            {
                throw new InvalidOperationException("A pergunta já existe no modelo.");
            }
            _perguntas.Add(pergunta);
        }
    }

    public void RemovePerguntas(IEnumerable<Pergunta> perguntas)
    {
        var perguntasParaRemover = _perguntas.Where(p => perguntas.Select(pr => pr.Id).Contains(p.Id)).ToList();

        foreach (Pergunta pergunta in perguntasParaRemover)
        {
            if (!_perguntas.Remove(pergunta))
            {
                throw new InvalidOperationException("A pergunta não existe no modelo.");
            }
        }
    }

    private void Validate()
    {
        DomainValidation.MaxLength(Titulo, 250, "Título");
    }

    public void UpdatePerguntas(IEnumerable<Pergunta> perguntasAtualizadas)
    {
        var perguntasAtualizadasIds = perguntasAtualizadas.Select(p => p.Id).ToList();  

        var perguntasParaRemover = _perguntas.Where(p => perguntasAtualizadasIds.Contains(p.Id) == false).ToList();

        RemovePerguntas(perguntasParaRemover);

        foreach (Pergunta perguntaAtualizada in perguntasAtualizadas)
        {
            Pergunta pergunta = _perguntas.SingleOrDefault(p => perguntaAtualizada.Id != Guid.Empty && p.Id == perguntaAtualizada.Id);

            if(pergunta is null)
            {
                _perguntas.Add(perguntaAtualizada);
            }
            else
            {
                pergunta.Update(perguntaAtualizada);
            }
        }
    }

    private Secao() { }
}
