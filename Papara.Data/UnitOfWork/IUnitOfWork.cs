using Microsoft.EntityFrameworkCore;
using Papara.Data.Entities;
using Papara.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task Complete();
        Task CompleteWithTransaction();
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<ProductCategory> ProductCategoryRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<OrderDetail> OrderDetailRepository { get; }
        IGenericRepository<Coupon> CouponRepository { get; }
    }
}
