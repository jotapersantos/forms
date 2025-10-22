using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Application.DTOs.Shared;
using CRM.Domain.Dtos.Formularios;

namespace CRM.Application.Interfaces.Formularios.Modelos;

public interface IModeloService
{
    Task<PagedResultDto<ModeloDTO>> GetPagedAsync(int page, int pageSize, string termo);
    Task<PagedResultDto<ModeloIndexDTO>> GetModeloIndexPagedAsync(int page, int pageSize, string termo);

    Task<ModeloDTO> GetByIdAsync(Guid id);
    Task<ModeloDTO> GetByIdWithTemaESecoesEPerguntasAsync(Guid id);
    Task<IEnumerable<PerguntaDTO>> GetAllRequiredPerguntasByModeloId(Guid modeloId);
    Task<Dictionary<Guid, string>> GetDictionaryActivesByTitleAsync(string title, int quantity);
    Task<bool> IsModeloAssociatedWithFormularioOrAvaliacaoAsync(Guid id);
    Task<bool> CreateAsync(ModeloDTO modeloDto);
    Task<bool> UpdateAsync(ModeloDTO modeloDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> Ativar(Guid id);
    Task<bool> Desativar(Guid id);
}
