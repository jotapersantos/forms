using System.Threading.Tasks;
using CRM.Domain.Interfaces;
using CRM.Infra.Data.Context;

namespace CRM.Infra.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly CRMDbContext _context;

    public Repository(CRMDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }
    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public async Task RemoveAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}
