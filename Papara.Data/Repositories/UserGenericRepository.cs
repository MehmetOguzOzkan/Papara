using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Papara.Data.Context;
using Papara.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.Repositories
{
    internal class UserGenericRepository<TUser> : IGenericRepository<TUser> where TUser : IdentityUser<Guid>
    {
        private readonly PaparaDbContext _context;

        public UserGenericRepository(PaparaDbContext context)
        {
            _context = context;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TUser?> GetById(Guid id, params string[] includes)
        {
            var query = _context.Set<TUser>().AsQueryable();
            query = includes.Aggregate(query, (current, inc) => current.Include(inc));
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TUser> Insert(TUser entity)
        {
            entity.Id = Guid.NewGuid();
            await _context.Set<TUser>().AddAsync(entity);
            return entity;
        }

        public void Update(TUser entity)
        {
            _context.Set<TUser>().Update(entity);
        }

        public void Delete(TUser entity)
        {
            _context.Set<TUser>().Remove(entity);
        }

        public async Task Delete(Guid id)
        {
            var entity = await _context.Set<TUser>().FindAsync(id);
            if (entity is not null)
                _context.Set<TUser>().Remove(entity);
        }

        public void SoftDelete(TUser entity)
        {
            entity.LockoutEnabled = true;
            Update(entity);
        }

        public async Task SoftDelete(Guid id)
        {
            var entity = await _context.Set<TUser>().FindAsync(id);
            if (entity is not null)
            {
                entity.LockoutEnabled = true;
                Update(entity);
            }
        }

        public async Task<IEnumerable<TUser>> Where(Expression<Func<TUser, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TUser>().Where(predicate).AsQueryable();
            query = includes.Aggregate(query, (current, inc) => current.Include(inc));
            return await query.ToListAsync();
        }

        public async Task<TUser> FirstOrDefault(Expression<Func<TUser, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TUser>().AsQueryable();
            query = includes.Aggregate(query, (current, inc) => current.Include(inc));
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TUser>> GetAll(params string[] includes)
        {
            var query = _context.Set<TUser>().AsQueryable();
            query = includes.Aggregate(query, (current, inc) => current.Include(inc));
            return await query.ToListAsync();
        }
    }
}
