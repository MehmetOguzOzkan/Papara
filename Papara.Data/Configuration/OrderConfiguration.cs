using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.Configuration
{
    internal class OrderConfiguration : BaseEntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Code).IsRequired().HasMaxLength(9);
            builder.Property(x => x.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.CouponAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.CouponCode).HasMaxLength(50);
            builder.Property(x => x.PointsUsed).HasColumnType("decimal(18,2)");
            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.IsPaid).HasDefaultValue(false);

            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId);
        }
    }
}
