using OrderDispatcher.OrderManagement.Core.Entities;
using System.Linq.Expressions;

namespace OrderDispatcher.OrderManagement.Core.DataAccess
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        T GetT(Expression<Func<T, bool>> filter = null);
        Task<T> GetTAsync(Expression<Func<T, bool>> filter = null);
        List<T> GetList(Expression<Func<T, bool>> filter = null);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> filter = null);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        T Update(T entity);
        Task<T> UpdateAsync(T entity);
        void Delete(T entity);
        Task DeleteAsync(T entity);
    }
}
