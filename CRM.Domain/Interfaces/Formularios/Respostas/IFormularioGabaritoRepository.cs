using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.Domain.Entities.Formularios.Respostas;

namespace CRM.Domain.Interfaces.Formularios.Respostas;

public interface IFormularioGabaritoRepository
{
    Task<IEnumerable<Guid>> GetAllFormularioGabaritosIdsByFormularioId(Guid formularioId);
    Task<FormularioGabarito> GetByIdWithRespostasAsync(Guid id);
}
