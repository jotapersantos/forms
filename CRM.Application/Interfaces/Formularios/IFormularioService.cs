using System;
using System.Threading.Tasks;
using CRM.Application.DTOs.Formularios;
using CRM.Application.DTOs.Formularios.Respostas;
using CRM.Application.DTOs.Shared;

namespace CRM.Application.Interfaces.Formularios;

public interface IFormularioService
{
    Task<bool> ResponderAsync(FormularioGabaritoDTO formularioGabarito);
    Task<PagedResultDto<FormularioDTO>> GetAllPagedAsync(int pagina, int tamanhoPagina, string termo);
    Task<FormularioDTO> GetByIdAsync(Guid id);
    Task<bool> CreateAsync(FormularioDTO formulario);
    Task<bool> UpdateAsync(FormularioDTO formulario);
    Task<bool> DeleteAsync(Guid id);
}
