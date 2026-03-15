using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valuto.AuditTrail.Abstractions.Model.Entities;
using Valuto.AuditTrail.Core;

namespace Valuto.AuditTrail.Worker.Persistence
{
    internal class AuditEntryMap : IEntityTypeConfiguration<AuditEntry>
    {
        public void Configure(EntityTypeBuilder<AuditEntry> builder)
        {
            builder.ToTable("AUDIT_TRAILS");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("ID")
                .HasColumnType("RAW(16)")
                .ValueGeneratedNever();

            builder.Property(e => e.TransactionId)
                .HasColumnName("TRANSACTION_ID")
                .HasColumnType("RAW(16)");

            builder.Property(e => e.Usuario).HasColumnName("USUARIO").HasMaxLength(255);
            builder.Property(e => e.Tipo).HasColumnName("TIPO").HasConversion<string>().HasMaxLength(50);
            builder.Property(e => e.DateUtc).HasColumnName("DATE_UTC");
            builder.Property(e => e.EntityName).HasColumnName("ENTITY_NAME").HasMaxLength(255);
            builder.Property(e => e.SistemaName).HasColumnName("SISTEMA_NAME").HasMaxLength(255);
            builder.Property(e => e.PrimaryKey).HasColumnName("PRIMARY_KEY").HasMaxLength(100);
            builder.Property(e => e.Root).HasColumnName("ROOT").HasMaxLength(255);
            builder.Property(e => e.RootId).HasColumnName("ROOT_ID").HasMaxLength(100);

            builder.Property(e => e.OldValues)
                .HasColumnName("OLD_VALUES")
                .HasJsonConversion("CLOB", "_oldValues");

            builder.Property(e => e.NewValues)
                .HasColumnName("NEW_VALUES")
                .HasJsonConversion("CLOB", "_newValues");

            builder.Property(e => e.ChangedColumns)
                .HasColumnName("CHANGED_COLUMNS")
                .HasJsonConversion("CLOB", "_changedColumns");

            builder.HasIndex(e => e.TransactionId).HasDatabaseName("IX_AUDIT_TRANSACTION_ID");
            builder.HasIndex(e => e.Id).IsUnique();
        }
    }
}
