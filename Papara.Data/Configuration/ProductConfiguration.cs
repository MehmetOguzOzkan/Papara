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
    internal class ProductConfiguration : BaseEntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.LoyaltyPointsRatio).IsRequired().HasColumnType("decimal(3,2)").HasPrecision(3, 2).HasDefaultValue(0);
            builder.Property(x => x.MaxPoints).IsRequired();
            builder.Property(x => x.InStock).HasDefaultValue(true);
        }
    }
}
