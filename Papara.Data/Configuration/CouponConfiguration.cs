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
    internal class CouponConfiguration : BaseEntityConfiguration<Coupon>
    {
        public override void Configure(EntityTypeBuilder<Coupon> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.Property(x => x.DiscountAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.ValidFrom).IsRequired();
            builder.Property(x => x.ValidTo).IsRequired();
            builder.Property(x => x.IsUsed).IsRequired();

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasOne(c => c.User).WithMany(u => u.Coupons).HasForeignKey(c => c.UserId);
        }
    }
}
