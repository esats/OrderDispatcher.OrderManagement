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

        protected EfEntityRepositoryBase()
        {
        }

        public TEntity Add(TEntity entity)
        {
            using var context = new TContext();
            var addedEntity = context.Entry(entity);
            addedEntity.State = EntityState.Added;
            context.SaveChanges();
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            using var context = new TContext();
            var addedEntity = context.Entry(entity);
            addedEntity.State = EntityState.Added;
            await context.SaveChangesAsync();
            return entity;
        }

        public void Delete(TEntity entity)
        {
            using var context = new TContext();
            var deletedEntity = context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            context.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            using var context = new TContext();
            var deletedEntity = context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            using var context = new TContext();
            return filter == null ? context.Set<TEntity>().ToList() : context.Set<TEntity>().Where(filter).ToList();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            await using var context = new TContext();
            var query = context.Set<TEntity>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public TEntity GetT(Expression<Func<TEntity, bool>> filter = null)
        {
            using var context = new TContext();
            return context.Set<TEntity>().SingleOrDefault(filter);
        }

        public async Task<TEntity> GetTAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            using var context = new TContext();
            return await context.Set<TEntity>().SingleOrDefaultAsync(filter);
        }

        public TEntity Update(TEntity entity)
        {
            using var context = new TContext();
            var updatedEntity = context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            context.SaveChanges();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            using var context = new TContext();
            var updatedEntity = context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }
    }
}
