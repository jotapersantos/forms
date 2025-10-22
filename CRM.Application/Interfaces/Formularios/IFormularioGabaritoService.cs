using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.Application.DTOs.Formularios.Respostas;

namespace CRM.Application.Interfaces.Formularios;

public interface IFormularioGabaritoService
{
    Task<IEnumerable<Guid>> GetAllFormularioGabaritosIdsByFormularioId(Guid formularioId);
    Task<FormularioGabaritoDTO> GetByIdWithRespostasAsync(Guid id);
}
