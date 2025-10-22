using System.Threading.Tasks;

namespace CRM.Domain.Interfaces;

public interface IUnitOfWork
{
    void Save();
    Task SaveAsync();
}

