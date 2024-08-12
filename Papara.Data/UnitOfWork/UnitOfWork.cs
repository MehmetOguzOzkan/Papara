using Papara.Data.Context;
using Papara.Data.Entities;
using Papara.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly PaparaDbContext _context;

        public IGenericRepository<User> UserRepository { get; private set; }
        public IGenericRepository<Product> ProductRepository { get; private set; }
        public IGenericRepository<Category> CategoryRepository { get; private set; }
        public IGenericRepository<ProductCategory> ProductCategoryRepository { get; private set; }
        public IGenericRepository<Order> OrderRepository { get; private set; }
        public IGenericRepository<OrderDetail> OrderDetailRepository { get; private set; }
        public IGenericRepository<Coupon> CouponRepository { get; private set; }

        public UnitOfWork(PaparaDbContext context)
        {
            _context = context;

            UserRepository = new UserGenericRepository<User>(_context);
            ProductRepository = new GenericRepository<Product>(_context);
            CategoryRepository = new GenericRepository<Category>(_context);
            ProductCategoryRepository = new GenericRepository<ProductCategory>(_context);
            OrderRepository = new GenericRepository<Order>(_context);
            OrderDetailRepository = new GenericRepository<OrderDetail>(_context);
            CouponRepository = new GenericRepository<Coupon>(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Complete()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CompleteWithTransaction()
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }
    }
}
