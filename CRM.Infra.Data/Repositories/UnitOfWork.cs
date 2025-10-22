using System.Threading.Tasks;
using CRM.Domain.Interfaces;
using CRM.Infra.Data.Context;

namespace CRM.Infra.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CRMDbContext _dbContext;

    public UnitOfWork(CRMDbContext dbContext) => _dbContext = dbContext;

    public void Save() => _dbContext.SaveChanges();

    public async Task SaveAsync() => await _dbContext.SaveChangesAsync();
}
