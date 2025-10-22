using System.Threading.Tasks;

namespace CRM.Domain.Interfaces;
public interface IRepository<T>
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
}
