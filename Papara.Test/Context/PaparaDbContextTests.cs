using Microsoft.EntityFrameworkCore;
using Moq;
using Papara.Data.Context;
using System;
using Xunit;

namespace Papara.Test.Context
{
    public class PaparaDbContextTests
    {
        private readonly DbContextOptions<PaparaDbContext> _options;

        public PaparaDbContextTests()
        {
            _options = new DbContextOptionsBuilder<PaparaDbContext>()
                            .UseInMemoryDatabase(databaseName: "PaparaTestDb")
                            .Options;
        }

        [Fact]
        public void Can_Create_DbContext()
        {
            using (var context = new PaparaDbContext(_options))
            {
                Assert.NotNull(context);
                Assert.NotNull(context.Products);
                Assert.NotNull(context.Categories);
                Assert.NotNull(context.ProductCategories);
                Assert.NotNull(context.Orders);
                Assert.NotNull(context.OrderDetails);
                Assert.NotNull(context.Coupons);
            }
        }
    }
}
