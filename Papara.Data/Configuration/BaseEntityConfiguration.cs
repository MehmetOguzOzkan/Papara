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
    public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.InsertDate).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdateDate).IsRequired(false).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        }
    }
}
