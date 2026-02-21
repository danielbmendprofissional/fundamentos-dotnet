using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valuto.AuditTrail.Worker.Model;

namespace Valuto.AuditTrail.Worker.Persistence
{
    internal class AuditLogsMap : IEntityTypeConfiguration<AuditLogs>
    {
        public void Configure(EntityTypeBuilder<AuditLogs> builder)
        {
            builder.ToTable("AUDIT_LOGS");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("ID")
                .HasColumnType("RAW(16)")
                .ValueGeneratedNever();


            builder.Property(e => e.Mensagem)
                .HasColumnName("MENSAGEM")
                .HasColumnType("CLOB");

            builder.Property(e => e.StackTrace)
                .HasColumnName("STACK_TRACE")
                .HasColumnType("CLOB");

            builder.Property(e => e.SistemaName).HasColumnName("SISTEMA_NAME").HasMaxLength(255);
            builder.Property(e => e.Contexto).HasColumnName("ENTITY_NAME").HasMaxLength(255);
            builder.Property(e => e.DateUtc).HasColumnName("DATE_UTC");
            builder.Property(e => e.VersaoNugget).HasColumnName("VERSAO_NUGGET").HasMaxLength(10);

            builder.HasIndex(e => e.Id).IsUnique();
        }
    }
}
