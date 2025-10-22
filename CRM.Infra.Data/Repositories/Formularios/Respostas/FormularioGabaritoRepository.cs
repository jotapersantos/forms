using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Domain.Entities.Formularios.Respostas;
using CRM.Domain.Interfaces.Formularios.Respostas;
using CRM.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infra.Data.Repositories.Formularios.Respostas;

public class FormularioGabaritoRepository : IFormularioGabaritoRepository
{
    private readonly CRMDbContext _contexto;

    public FormularioGabaritoRepository(CRMDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<Guid>> GetAllFormularioGabaritosIdsByFormularioId(Guid formularioId)
        => await _contexto.FormularioGabaritos
                          .Where(gabarito => gabarito.FormularioId == formularioId)
                          .Select(gabarito => gabarito.Id)
                          .ToListAsync();

    public async Task<FormularioGabarito> GetByIdWithRespostasAsync(Guid id)
        => await _contexto.FormularioGabaritos
                          .Include(gabarito => gabarito.Respostas)
                          .ThenInclude(resposta => (resposta as RespostaObjetiva).AlternativasSelecionadas)
                          .FirstOrDefaultAsync(gabarito => gabarito.Id == id);
}
