using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios.Modelos;

public sealed class Modelo : Entity
{
    public string Titulo { get; private set; }
    public DateTime CriadoEmUtc { get; private set; }
    public bool Ativo { get; private set; }

    private readonly List<Secao> _secoes = [];
    public IReadOnlyCollection<Secao> Secoes => _secoes.AsReadOnly();
    
    private Modelo(string titulo, bool ativo = true)
    {
        Titulo = titulo;
        CriadoEmUtc = DateTime.UtcNow;
        Ativo = ativo;

        Validate();
    }

    public static Modelo Build(string titulo, bool ativo = true)
    {
        return new Modelo(titulo, ativo);
    }

    public void Update(string titulo, bool ativo = true)
    {
        Titulo = titulo;
        Ativo= ativo;

        Validate();
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;    

    public void AddSecoes(IEnumerable<Secao> secoes)
    {
        if(!secoes.Any())
        {
            throw new InvalidOperationException("O modelo deve ter no mínimo uma seção");
        }

        foreach (Secao secao in secoes)
        {
            if (_secoes.Any(s => s.Id == secao.Id) && secao.Id != Guid.Empty)
            {
                throw new InvalidOperationException("A seção já está atribuída ao formulário.");
            }

            _secoes.Add(secao);
        }
    }

    public void UpdateSecoes(IEnumerable<Secao> secoesAtualizadas)
    {
        var secoesAtualizadasIds = secoesAtualizadas.Select(s => s.Id).ToList();

        var secoesParaRemover = _secoes.Where(s => secoesAtualizadasIds.Contains(s.Id) == false).ToList();

        RemoveSecoes(secoesParaRemover);

        foreach(Secao secaoAtualizada in secoesAtualizadas)
        {
            Secao secao = _secoes.SingleOrDefault(s => secaoAtualizada.Id != Guid.Empty && s.Id == secaoAtualizada.Id);

            if(secao is null)
            {
                _secoes.Add(secaoAtualizada);
            }
            else
            {
                secao.Update(secaoAtualizada.Titulo, secaoAtualizada.Ordem);
                secao.UpdatePerguntas(secaoAtualizada.Perguntas);
            }
        }
    }

    public void RemoveSecoes(IEnumerable<Secao> secoes)
    {
        var secoesParaRemover = _secoes.Where(s => secoes.Select(sr => sr.Id).Contains(s.Id)).ToList();

        foreach (Secao secao in secoesParaRemover)
        {
            if (!_secoes.Remove(secao))
            {
                throw new InvalidOperationException("A seção não está atribuída ao formulário.");
            }
        }
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Titulo, "Título");
        DomainValidation.MinLength(Titulo, 3, "Título");
        DomainValidation.MaxLength(Titulo, 250, "Título");
    }

    private Modelo() { }
}
