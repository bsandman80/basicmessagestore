using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasicMessageStore.Models
{
    /// <summary>
    /// Async interface for a generic repository
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T model);
        Task DeleteAsync(int id);
        Task UpdateAsync(T model);
        Task<IEnumerable<T>> GetAsync();
        Task<T> GetByIdAsync(int id);
    }
}    