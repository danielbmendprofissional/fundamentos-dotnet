using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence
{
    internal abstract class EntityMap<TEntity>
        : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(e => e.DateCreated)
                .HasColumnName("date_created")
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(e => e.DateUpdated)
                .HasColumnName("date_updated")
                .HasColumnType("timestamptz");

            ConfigureMapping(builder);
        }

        protected abstract void ConfigureMapping(EntityTypeBuilder<TEntity> builder);
    }
}
