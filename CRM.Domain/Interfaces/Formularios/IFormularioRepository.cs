using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.Domain.Entities.Formularios;

namespace CRM.Domain.Interfaces.Formularios;

public interface IFormularioRepository
{
    Task<List<Formulario>> GetAllPagedAsync(int pagina, int tamanhoPagina, string termo);
    Task<Formulario> GetByIdAsync(Guid id);
    Task<int> CountAsync(string termo);
    Task AddAsync(Formulario formulario);
    void Update(Formulario formulario);
    void Remove(Formulario formulario);
}
