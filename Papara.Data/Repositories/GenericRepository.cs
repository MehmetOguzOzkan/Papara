using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Papara.Data.Context;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly PaparaDbContext _context;

        public GenericRepository(PaparaDbContext context)
        {
            _context = context;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity?> GetById(Guid id, params string[] includes)
        {
            var query = _context.Set<TEntity>().Where(x => x.IsActive).AsQueryable();
            query = includes.Aggregate(query, (current, inc) => EntityFrameworkQueryableExtensions.Include(current, inc));
            return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(query, x => x.Id == id);
        }

        public async Task<TEntity> Insert(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public void Update(TEntity entity)
        {
            entity.UpdateDate = DateTime.UtcNow;
            _context.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public async Task Delete(Guid id)
        {
            var entity = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.Set<TEntity>(), x => x.Id == id);
            if (entity is not null)
                _context.Set<TEntity>().Remove(entity);
        }

        public void SoftDelete(TEntity entity)
        {
            entity.IsActive = false;
            Update(entity);
        }

        public async Task SoftDelete(Guid id)
        {
            var entity = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.Set<TEntity>(), x => x.Id == id);
            if (entity is not null)
            {
                entity.IsActive = false;
                Update(entity);
            }
        }

        public async Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TEntity>().Where(x=>x.IsActive).Where(predicate).AsQueryable();
            query = includes.Aggregate(query, (current, inc) => EntityFrameworkQueryableExtensions.Include(current, inc));
            return await EntityFrameworkQueryableExtensions.ToListAsync(query);
        }

        public async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TEntity>().Where(x => x.IsActive).AsQueryable();
            query = includes.Aggregate(query, (current, inc) => EntityFrameworkQueryableExtensions.Include(current, inc));
            return query.Where(predicate).FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> GetAll(params string[] includes)
        {
            var query = _context.Set<TEntity>().Where(x => x.IsActive).AsQueryable();
            query = includes.Aggregate(query, (current, inc) => EntityFrameworkQueryableExtensions.Include(current, inc));
            return await EntityFrameworkQueryableExtensions.ToListAsync(query);
        }
    }
}
