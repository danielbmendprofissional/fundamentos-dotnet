using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valuto.AuditTrail.Test6
{
    internal class SOLICITACAOMap : IEntityTypeConfiguration<SOLICITACAO>
    {
        public void Configure(EntityTypeBuilder<SOLICITACAO> builder)
        {
            builder.HasKey(t => t.COD_SOLICITACAO);

            // Properties
            builder.Property(t => t.COD_SOLICITACAO)
                .ValueGeneratedOnAdd();


            builder.Property(t => t.NUM_PROTO_EXTERNO)
                .HasMaxLength(4);

            builder.Property(t => t.NUM_PROTOCOLO)
                .HasMaxLength(16);
            //.IsRequired();

            builder.Property(t => t.IND_RESP_PARC_PEND)
                .HasMaxLength(1);

            builder.Property(t => t.IND_FECHADO)
                .IsRequired();

            builder.Property(t => t.DSC_SATISFACAO)
                .HasMaxLength(1000);

            builder.Property(t => t.DSC_SATISFACAO_HTML)
                .HasMaxLength(1000);

            builder.Property(t => t.DSC_TEMPO_FILA)
                .HasMaxLength(100);

            builder.Property(t => t.NOM_ULT_RESPONSAVEL)
                .HasMaxLength(200);

            builder.Property(t => t.NOM_ULT_DEPTO_RESP)
                .HasMaxLength(200);

            builder.Property(t => t.IND_VENCE_HOJE)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("SOLICITACAO");
            builder.Property(t => t.COD_SOLICITACAO).HasColumnName("COD_SOLICITACAO");
            builder.Property(t => t.NUM_SOLICITACAO).HasColumnName("NUM_SOLICITACAO");
            builder.Property(t => t.NUM_PROTO_EXTERNO).HasColumnName("NUM_PROTO_EXTERNO");
            builder.Property(t => t.DAT_CADASTRO).HasColumnName("DAT_CADASTRO");
            builder.Property(t => t.IND_RESP_PARC_PEND).HasColumnName("IND_RESP_PARC_PEND");
            builder.Property(t => t.COD_IND_TIPO).HasColumnName("COD_IND_TIPO");
            builder.Property(t => t.COD_IND_FORMA).HasColumnName("COD_IND_FORMA");
            builder.Property(t => t.COD_IND_SATISFACAO).HasColumnName("COD_IND_SATISFACAO");
            builder.Property(t => t.DSC_SATISFACAO).HasColumnName("DSC_SATISFACAO");
            builder.Property(t => t.DSC_SATISFACAO_HTML).HasColumnName("DSC_SATISFACAO_HTML");
            builder.Property(t => t.NUM_PROTOCOLO).HasColumnName("NUM_PROTOCOLO");
            builder.Property(t => t.IND_FECHADO).HasColumnName("IND_FECHADO");
            builder.Property(t => t.NUM_DIAS_ATRASO).HasColumnName("NUM_DIAS_ATRASO");
            builder.Property(t => t.IND_VENCE_HOJE).HasColumnName("IND_VENCE_HOJE");
            builder.Property(t => t.COD_IND_CANAL).HasColumnName("COD_IND_CANAL");
            builder.Property(t => t.COD_ULT_RESPONSAVEL).HasColumnName("COD_ULT_RESPONSAVEL");
            builder.Property(t => t.NOM_ULT_RESPONSAVEL).HasColumnName("NOM_ULT_RESPONSAVEL");
            builder.Property(t => t.COD_ULT_DEPTO_RESP).HasColumnName("COD_ULT_DEPTO_RESP");
            builder.Property(t => t.NOM_ULT_DEPTO_RESP).HasColumnName("NOM_ULT_DEPTO_RESP");
            builder.Property(t => t.DSC_TEMPO_FILA).HasColumnName("DSC_TEMPO_FILA");
            builder.Property(t => t.NUM_TEMPO_FILA).HasColumnName("NUM_TEMPO_FILA");
            builder.Property(t => t.COD_OCORRENCIA_LEGADO).HasColumnName("COD_OCORRENCIA_LEGADO");
            builder.Property(t => t.DSC_ABERTURA).HasColumnName("DSC_ABERTURA");
            builder.Property(t => t.DSC_ABERTURA_HTML).HasColumnName("DSC_ABERTURA_HTML");

            builder.Property(t => t.COD_ULT_SIT_TRAMITE).HasColumnName("COD_ULT_SIT_TRAMITE");
            builder.Property(t => t.DSC_ULT_SIT_TRAMITE).HasColumnName("DSC_ULT_SIT_TRAMITE");
            builder.Property(t => t.COD_DEPTO_ORIGEM).HasColumnName("COD_DEPTO_ORIGEM");
            builder.Property(t => t.NOM_DEPTO_ORIGEM).HasColumnName("NOM_DEPTO_ORIGEM");
            builder.Property(t => t.COD_USU_DEMANDANTE).HasColumnName("COD_USU_DEMANDANTE");
            builder.Property(t => t.NOM_USU_DEMANDANTE).HasColumnName("NOM_USU_DEMANDANTE");
            builder.Property(t => t.IND_EXIBE_RESPONSAVEL_SOLICIT).HasColumnName("IND_EXIBE_RESPONSAVEL_SOLICIT");
            builder.Property(t => t.IND_ENVIO_MSG_WHATSAPP).HasColumnName("IND_ENVIO_MSG_WHATSAPP");

            builder.HasMany(t => t.SOLICITACAO_ANEXO)
                .WithOne(a => a.SOLICITACAO)
                .HasForeignKey(a => a.COD_SOLICITACAO)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.SOLICITACAO_HISTORICO_STATUS)
                .WithOne(h => h.SOLICITACAO)
                .HasForeignKey(h => h.COD_SOLICITACAO);
        }
    }

    internal class SOLICITACAO_TRAMITEMap : IEntityTypeConfiguration<SOLICITACAO_TRAMITE>
    {
        public void Configure(EntityTypeBuilder<SOLICITACAO_TRAMITE> builder)
        {
            // Primary Key
            builder.HasKey(t => t.COD_SOLICIT_TRAMITE);

            // Properties
            builder.Property(t => t.COD_SOLICIT_TRAMITE)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.IND_RESOLVIDO)
                .IsRequired()
                .HasMaxLength(1);

            builder.Property(t => t.COD_IND_SITUACAO)
                .IsRequired();

            builder.Property(t => t.COD_IND_TRAMITACAO)
                .IsRequired();

            builder.Property(t => t.DSC_TRAMITE)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(t => t.DSC_TRAMITE_HTML)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(t => t.NOM_USU_SIGESP)
                .HasMaxLength(200);

            builder.Property(t => t.NOM_USU_SGI)
                .HasMaxLength(200);

            builder.Property(t => t.DSC_LOTACAO_SIGESP)
                .HasMaxLength(200);

            builder.Property(t => t.DAT_CADASTRO).IsRequired();

            builder.Property(t => t.DSC_DIAS_TRAMITE)
                .HasMaxLength(4000);

            // Table & Column Mappings
            builder.ToTable("SOLICITACAO_TRAMITE");
            builder.Property(t => t.COD_SOLICIT_TRAMITE).HasColumnName("COD_SOLICIT_TRAMITE");
            builder.Property(t => t.COD_SOLICITACAO).HasColumnName("COD_SOLICITACAO");
            builder.Property(t => t.IND_RESOLVIDO).HasColumnName("IND_RESOLVIDO");
            builder.Property(t => t.COD_IND_SITUACAO).HasColumnName("COD_IND_SITUACAO");
            builder.Property(t => t.COD_IND_TRAMITACAO).HasColumnName("COD_IND_TRAMITACAO");
            builder.Property(t => t.DSC_TRAMITE).HasColumnName("DSC_TRAMITE");
            builder.Property(t => t.COD_DOC_ANEXO).HasColumnName("COD_DOC_ANEXO");
            builder.Property(t => t.COD_USU_SIGESP).HasColumnName("COD_USU_SIGESP");
            builder.Property(t => t.NOM_USU_SIGESP).HasColumnName("NOM_USU_SIGESP");
            builder.Property(t => t.COD_LOTACAO_SIGESP).HasColumnName("COD_LOTACAO_SIGESP");
            builder.Property(t => t.DSC_LOTACAO_SIGESP).HasColumnName("DSC_LOTACAO_SIGESP");
            builder.Property(t => t.COD_CIDADAO).HasColumnName("COD_CIDADAO");
            builder.Property(t => t.COD_ENC_SOLICIT_TRAMITE).HasColumnName("COD_ENC_SOLICIT_TRAMITE");
            builder.Property(t => t.DAT_REAGENDAMENTO).HasColumnName("DAT_REAGENDAMENTO");
            builder.Property(t => t.DAT_CADASTRO).HasColumnName("DAT_CADASTRO");
            builder.Property(t => t.COD_USU_SGI).HasColumnName("COD_USU_SGI");
            builder.Property(t => t.NOM_USU_SGI).HasColumnName("NOM_USU_SGI");
            builder.Property(t => t.DSC_TRAMITE_HTML).HasColumnName("DSC_TRAMITE_HTML");
            builder.Property(t => t.COD_PESSOA_OUV).HasColumnName("COD_PESSOA_OUV");
            builder.Property(t => t.COD_USU_INCLUSAO).HasColumnName("COD_USU_INCLUSAO");
            builder.Property(t => t.DSC_CONCLUSAO_TECNICA).HasColumnName("DSC_CONCLUSAO_TECNICA");
            builder.Property(t => t.COD_IND_CANAL).HasColumnName("COD_IND_CANAL");
            builder.Property(t => t.COD_EQUIPE).HasColumnName("COD_EQUIPE");
            builder.Property(t => t.DSC_DEPTO_NOTIFICADOS).HasColumnName("DSC_DEPTO_NOTIFICADOS");
            builder.Property(t => t.DSC_DIAS_TRAMITE).HasColumnName("DSC_DIAS_TRAMITE");
            builder.Property(t => t.IND_NOTIF_WP).HasColumnName("IND_NOTIF_WP");
            builder.Property(t => t.COD_REQUEST_WPP).HasColumnName("COD_REQUEST_WPP");

            // Relationships

            builder.HasOne(t => t.SOLICITACAO)
                   .WithMany(s => s.SOLICITACAO_TRAMITE)
                   .HasForeignKey(t => t.COD_SOLICITACAO)
                   .IsRequired();

            builder.HasMany(t => t.COMENTARIOS)
                .WithOne(c => c.SOLICITACAO_TRAMITE)
                .HasForeignKey(c => c.COD_SOLICIT_TRAMITE);

            builder.HasMany(t => t.LOGS)
                .WithOne(l => l.SOLICITACAO_TRAMITE)
                .HasForeignKey(l => l.COD_SOLICIT_TRAMITE);
        }
    }

    internal class SOLICITACAO_ANEXOMap : IEntityTypeConfiguration<SOLICITACAO_ANEXO>
    {
        public void Configure(EntityTypeBuilder<SOLICITACAO_ANEXO> builder)
        {
            builder.ToTable("SOLICITACAO_ANEXO");

            builder.HasKey(t => t.COD_ANEXO);
            builder.Property(t => t.COD_ANEXO).ValueGeneratedOnAdd();

            builder.Property(t => t.NOM_ARQUIVO).HasMaxLength(255).IsRequired();
            builder.Property(t => t.DSC_EXTENSAO).HasMaxLength(10).IsRequired();

            builder.HasOne(t => t.SOLICITACAO)
                   .WithMany(s => s.SOLICITACAO_ANEXO)
                   .HasForeignKey(t => t.COD_SOLICITACAO);
        }
    }

    internal class SOLICITACAO_HISTORICO_STATUSMap : IEntityTypeConfiguration<SOLICITACAO_HISTORICO_STATUS>
    {
        public void Configure(EntityTypeBuilder<SOLICITACAO_HISTORICO_STATUS> builder)
        {
            builder.ToTable("SOLICITACAO_HISTORICO_STATUS");

            builder.HasKey(t => t.COD_HISTORICO);
            builder.Property(t => t.COD_HISTORICO).ValueGeneratedOnAdd();

            builder.Property(t => t.DAT_ALTERACAO).IsRequired();

            builder.HasOne(t => t.SOLICITACAO)
                   .WithMany(s => s.SOLICITACAO_HISTORICO_STATUS)
                   .HasForeignKey(t => t.COD_SOLICITACAO);
        }
    }

    internal class SOLICITACAO_TRAMITE_COMENTARIOMap : IEntityTypeConfiguration<SOLICITACAO_TRAMITE_COMENTARIO>
    {
        public void Configure(EntityTypeBuilder<SOLICITACAO_TRAMITE_COMENTARIO> builder)
        {
            builder.ToTable("SOLICITACAO_TRAMITE_COMENTARIO");

            builder.HasKey(t => t.COD_COMENTARIO);
            builder.Property(t => t.COD_COMENTARIO).ValueGeneratedOnAdd();

            builder.Property(t => t.TXT_COMENTARIO).HasMaxLength(4000).IsRequired();

            builder.HasOne(t => t.SOLICITACAO_TRAMITE)
                   .WithMany(s => s.COMENTARIOS)
                   .HasForeignKey(t => t.COD_SOLICIT_TRAMITE);
        }
    }

    internal class SOLICITACAO_TRAMITE_LOGMap : IEntityTypeConfiguration<SOLICITACAO_TRAMITE_LOG>
    {
        public void Configure(EntityTypeBuilder<SOLICITACAO_TRAMITE_LOG> builder)
        {
            // Table & Column Mappings
            builder.ToTable("SOLICITACAO_TRAMITE_LOG");

            // Primary Key
            builder.HasKey(t => t.COD_LOG_TRAMITE);

            // Properties
            builder.Property(t => t.COD_LOG_TRAMITE)
                .ValueGeneratedOnAdd()
                .HasColumnName("COD_LOG_TRAMITE");

            builder.Property(t => t.COD_SOLICIT_TRAMITE)
                .IsRequired()
                .HasColumnName("COD_SOLICIT_TRAMITE");

            builder.Property(t => t.DSC_LOG)
                .IsRequired()
                .HasMaxLength(4000)
                .HasColumnName("DSC_LOG");

            builder.HasOne(t => t.SOLICITACAO_TRAMITE)
                   .WithMany(s => s.LOGS)
                   .HasForeignKey(t => t.COD_SOLICIT_TRAMITE)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
