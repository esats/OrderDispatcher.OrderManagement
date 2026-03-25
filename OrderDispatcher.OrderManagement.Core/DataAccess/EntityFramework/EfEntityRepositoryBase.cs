using Microsoft.EntityFrameworkCore;
using OrderDispatcher.OrderManagement.Core.DataAccess;
using OrderDispatcher.OrderManagement.Core.Entities;
using System.Linq.Expressions;

namespace OrderDispatcher.OrderManagement.Core.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext, new()
    {
        protected readonly TContext _context;

        protected EfEntityRepositoryBase(TContext context)
        {
            _context = context;
        }

        public TEntity Add(TEntity entity)
        {
            var addedEntity = _context.Entry(entity);
            addedEntity.State = EntityState.Added;
            _context.SaveChanges();
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var addedEntity = _context.Entry(entity);
            addedEntity.State = EntityState.Added;
            await _context.SaveChangesAsync();
            return entity;
        }

        public void Delete(TEntity entity)
        {
            var deletedEntity = _context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            var deletedEntity = _context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null ? _context.Set<TEntity>().ToList() : _context.Set<TEntity>().Where(filter).ToList();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public TEntity GetT(Expression<Func<TEntity, bool>> filter = null)
        {
            return _context.Set<TEntity>().SingleOrDefault(filter);
        }

        public async Task<TEntity> GetTAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await _context.Set<TEntity>().SingleOrDefaultAsync(filter);
        }

        public TEntity Update(TEntity entity)
        {
            var updatedEntity = _context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            _context.SaveChanges();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var updatedEntity = _context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
