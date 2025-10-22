using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Domain.Dtos.Formularios;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Interfaces.Formularios.Modelos;
using CRM.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infra.Data.Repositories.Formularios.Modelos;

public class ModeloRepository : IModeloRepository
{
    private readonly CRMDbContext _contexto;

    public ModeloRepository(CRMDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Modelo>> GetPagedAsync(int pagina, int tamanhoPagina, string termo)
    {
        IQueryable<Modelo> query = _contexto.Modelos
            .AsNoTracking()
            .OrderBy(p => p.CriadoEmUtc)
            .AsQueryable();

        if (string.IsNullOrEmpty(termo))
        {
            return await query.Skip(tamanhoPagina * (pagina - 1))
                              .Take(tamanhoPagina)
                              .ToListAsync();
        }

        return await query.Where(formulario => formulario.Titulo.ToLower().Contains(termo.ToLower()))
                          .Skip(tamanhoPagina * (pagina - 1))
                          .Take(tamanhoPagina)
                          .ToListAsync();
    }

    public async Task<Dictionary<Guid, string>> GetDictionaryActivesByTitleAsync(string title, int quantity)
    {
        IQueryable<Modelo> query = _contexto.Modelos.AsNoTracking()
                                                    .Where(m => m.Ativo)
                                                    .OrderByDescending(p => p.CriadoEmUtc)
                                                    .AsQueryable();

        if (string.IsNullOrEmpty(title))
        {
            return await query.Take(quantity).ToDictionaryAsync(modelo => modelo.Id, modelo => modelo.Titulo);
        }

        return await query.Where(modelo => modelo.Titulo.ToLower().Contains(title.ToLower()))
                          .Take(quantity)
                          .ToDictionaryAsync(modelo => modelo.Id, modelo => modelo.Titulo);
    }

    public async Task<Modelo> GetByIdAsync(Guid id)
        => await _contexto.Modelos.AsNoTracking()
                                  .SingleOrDefaultAsync(p => p.Id == id);

    public async Task<Modelo> GetActivedByIdAsync(Guid id)
        => await _contexto.Modelos
                      .AsNoTracking()
                      .SingleOrDefaultAsync(m => m.Ativo && m.Id == id);

    public async Task<Modelo> GetByIdWithTemaESecoesEPerguntasAsync(Guid id)
    {
        Modelo modelo = await _contexto.Modelos
                                       .Include(modelo => modelo.Secoes.OrderBy(secao => secao.Ordem))
                                           .ThenInclude(modeloSecao => modeloSecao.Perguntas.OrderBy(pergunta => pergunta.Ordem))
                                       .FirstOrDefaultAsync(modelo => modelo.Id == id);

        foreach (Secao secao in modelo.Secoes)
        {
            foreach (Pergunta item in secao.Perguntas)
            {
                if (item is PerguntaObjetiva itemComOpcoes)
                {
                    await _contexto.Entry(itemComOpcoes)
                                   .Collection(i => i.Alternativas)
                                   .Query()
                                   .OrderBy(o => o.Ordem)
                                   .LoadAsync();
                }
            }
        }

        return modelo;
    }

    public async Task<Modelo> GetByIdWithThemeAsync(Guid id)
        => await _contexto.Modelos.AsNoTracking()
                                  .FirstOrDefaultAsync(modelo => modelo.Id == id);

    public async Task<IEnumerable<Pergunta>> GetAllRequiredPerguntasByModeloId(Guid modeloId)
    {
        IEnumerable<Pergunta> formularioModeloPerguntas = await _contexto.Modelos
                                                                         .Where(modelo => modelo.Id == modeloId)
                                                                         .Include(modelo => modelo.Secoes)
                                                                         .ThenInclude(modelo => modelo.Perguntas)
                                                                         .SelectMany(modelo => modelo.Secoes)
                                                                         .SelectMany(secao => secao.Perguntas.Where(modelo => modelo.Obrigatorio == true))
                                                                         .OrderBy(item => item.Ordem)
                                                                         .ToListAsync();


        foreach (Pergunta item in formularioModeloPerguntas)
        {
            if (item is PerguntaObjetiva itemComOpcoes)
            {
                _contexto.Entry(itemComOpcoes)
                    .Collection(i => i.Alternativas)
                    .Query()
                    .OrderBy(o => o.Ordem)
                    .Load();
            }
        }

        return formularioModeloPerguntas;
    }


    public async Task<IEnumerable<Pergunta>> GetAllPerguntasByModeloId(Guid modeloId)
    {
        IEnumerable<Pergunta> formularioModeloPerguntas = await _contexto.Modelos
                                                                         .Where(modelo => modelo.Id == modeloId)
                                                                         .Include(modelo => modelo.Secoes)
                                                                         .ThenInclude(modelo => modelo.Perguntas)
                                                                         .SelectMany(modelo => modelo.Secoes)
                                                                         .SelectMany(secao => secao.Perguntas)
                                                                         .OrderBy(item => item.Ordem)
                                                                         .ToListAsync();


        foreach (Pergunta item in formularioModeloPerguntas)
        {
            if (item is PerguntaObjetiva itemComOpcoes)
            {
                _contexto.Entry(itemComOpcoes)
                    .Collection(i => i.Alternativas)
                    .Query()
                    .OrderBy(o => o.Ordem)
                    .Load();
            }
        }

        return formularioModeloPerguntas;
    }

    public async Task<int> CountAsync(int pagina, int tamanhoPagina, string termo)
    {
        IQueryable<Modelo> query = _contexto.Modelos.AsNoTracking().AsQueryable();

        if (string.IsNullOrEmpty(termo))
        {
            return await query.CountAsync();
        }

        query = query.Where(formulario => formulario.Titulo.ToLower().Contains(termo.ToLower()));

        return await query.CountAsync();
    }

    public async Task<int> GetTotalPerguntasInSectionFormsByFormIdAsync(Guid id)
        => await _contexto.Modelos.AsNoTracking()
                                  .Where(p => p.Id == id)
                                  .Include(p => p.Secoes)
                                  .ThenInclude(s => s.Perguntas)
                                  .SelectMany(p => p.Secoes)
                                  .SelectMany(s => s.Perguntas)
                                  .CountAsync();

    public async Task AddAsync(Modelo modelo) =>
       await _contexto.Modelos.AddAsync(modelo);

    public void Update(Modelo modelo) =>
        _contexto.Modelos.Update(modelo);

    public void Remove(Modelo modelo) =>
        _contexto.Modelos.Remove(modelo);

    public async Task<IEnumerable<ModeloIndexDTO>> GetModeloIndexPagedAsync(int pagina, int tamanhoPagina, string termo)
        => await _contexto.Modelos
                          .AsNoTracking()
                          .Select(modelo => new ModeloIndexDTO
                          {
                              Id = modelo.Id,
                              Titulo = modelo.Titulo,
                              Ativo = modelo.Ativo,
                              ModeloUtilizado = _contexto.Formularios
                                                            .Where(formulario => formulario.ModeloId ==  modelo.Id) 
                                                            .Select(formulario => formulario.Id)
                                                            .Any()
                          }).ToListAsync();

    public async Task<bool> IsModeloAssociatedWithFormularioOrAvaliacaoAsync(Guid id)
        => await _contexto.Formularios
                          .Where(formulario => formulario.ModeloId == id)
                          .Select(formulario => formulario.Id)
                          .AnyAsync();
}
