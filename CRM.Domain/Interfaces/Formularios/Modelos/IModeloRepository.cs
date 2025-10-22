using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.Domain.Dtos.Formularios;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;

namespace CRM.Domain.Interfaces.Formularios.Modelos;

public interface IModeloRepository
{
    Task<IEnumerable<Modelo>> GetPagedAsync(int pagina, int tamanhoPagina, string termo);
    Task<IEnumerable<ModeloIndexDTO>> GetModeloIndexPagedAsync(int pagina, int tamanhoPagina, string termo);
    Task<bool> IsModeloAssociatedWithFormularioOrAvaliacaoAsync(Guid id);
    Task<Dictionary<Guid, string>> GetDictionaryActivesByTitleAsync(string title, int quantity);
    Task<Modelo> GetByIdAsync(Guid id);
    Task<Modelo> GetActivedByIdAsync(Guid id);
    Task<Modelo> GetByIdWithTemaESecoesEPerguntasAsync(Guid id);
    Task<Modelo> GetByIdWithThemeAsync(Guid id);
    Task<IEnumerable<Pergunta>> GetAllRequiredPerguntasByModeloId(Guid modeloId);
    Task<IEnumerable<Pergunta>> GetAllPerguntasByModeloId(Guid modeloId);
    Task<int> CountAsync(int pagina, int tamanhoPagina, string termo);
    Task<int> GetTotalPerguntasInSectionFormsByFormIdAsync(Guid id);
    Task AddAsync(Modelo modelo);
    void Update(Modelo modelo);
    void Remove(Modelo modelo);
}
