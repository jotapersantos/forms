using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Domain.Entities.Formularios;
using CRM.Domain.Interfaces.Formularios;
using CRM.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infra.Data.Repositories.Formularios;

public class FormularioRepository : IFormularioRepository
{
    private readonly CRMDbContext _contexto;

    public FormularioRepository(CRMDbContext contexto) =>
        _contexto = contexto;

    public async Task<List<Formulario>> GetAllPagedAsync(int pagina, int tamanhoPagina, string termo)
    {
        IQueryable<Formulario> query = _contexto.Formularios.AsNoTracking()
                                                            .Include(formulario => formulario.Modelo)
                                                            .AsQueryable();

        if (string.IsNullOrEmpty(termo))
        {
            return await query.OrderByDescending(formulario => formulario.Periodo.DataInicio)
                              .Skip(tamanhoPagina * (pagina - 1))
                              .Take(tamanhoPagina)
                              .ToListAsync();
        }

        return await query.Where(formulario => formulario.Nome.ToLower().Contains(termo.ToLower()))
                          .OrderByDescending(formulario => formulario.Periodo.DataInicio)
                          .Skip(tamanhoPagina * (pagina - 1))
                          .Take(tamanhoPagina)
                          .ToListAsync();
    }

    public async Task<Formulario> GetByIdAsync(Guid id)
        => await _contexto.Formularios
                          .Include(formulario => formulario.Gabaritos)
                          .FirstOrDefaultAsync(formulario => formulario.Id == id);

    public async Task<int> CountAsync(string termo)
    {
        IQueryable<Formulario> query = _contexto.Formularios.AsNoTracking()
                                                            .AsQueryable();

        if (string.IsNullOrEmpty(termo))
        {
            return await query.CountAsync();
        }

        query = query.Where(formulario => formulario.Nome.ToLower().Contains(termo.ToLower()));

        return await query.CountAsync();
    }

    public async Task AddAsync(Formulario formulario)
        => await _contexto.Formularios.AddAsync(formulario);

    public void Update(Formulario formulario)
        => _contexto.Formularios.Update(formulario);

    public void Remove(Formulario formulario)
        => _contexto.Formularios.Remove(formulario);

}
