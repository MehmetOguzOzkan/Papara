using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task Save();
        Task<TEntity?> GetById(Guid id, params string[] includes);
        Task<TEntity> Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task Delete(Guid id);
        void SoftDelete(TEntity entity);
        Task SoftDelete(Guid id);
        Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> predicate, params string[] includes);
        Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate, params string[] includes);
        Task<IEnumerable<TEntity>> GetAll(params string[] includes);

    }
}
