using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Valuto.AuditTrail.TestEF
{
    internal class SOLICITACAOMap : EntityTypeConfiguration<SOLICITACAO>
    {
        public SOLICITACAOMap()
        {
            this.HasKey(t => t.COD_SOLICITACAO);

            // Properties
            this.Property(t => t.COD_SOLICITACAO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            this.Property(t => t.NUM_PROTO_EXTERNO)
                .HasMaxLength(4);

            this.Property(t => t.NUM_PROTOCOLO)
                .HasMaxLength(16);
            //.IsRequired();

            this.Property(t => t.IND_RESP_PARC_PEND)
                .HasMaxLength(1);

            this.Property(t => t.IND_FECHADO)
                .IsRequired();

            this.Property(t => t.DSC_SATISFACAO)
                .HasMaxLength(1000);

            this.Property(t => t.DSC_SATISFACAO_HTML)
                .HasMaxLength(1000);

            this.Property(t => t.DSC_TEMPO_FILA)
                .HasMaxLength(100);

            this.Property(t => t.NOM_ULT_RESPONSAVEL)
                .HasMaxLength(200);

            this.Property(t => t.NOM_ULT_DEPTO_RESP)
                .HasMaxLength(200);

            this.Property(t => t.IND_VENCE_HOJE)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SOLICITACAO");
            this.Property(t => t.COD_SOLICITACAO).HasColumnName("COD_SOLICITACAO");
            this.Property(t => t.NUM_SOLICITACAO).HasColumnName("NUM_SOLICITACAO");
            this.Property(t => t.NUM_PROTO_EXTERNO).HasColumnName("NUM_PROTO_EXTERNO");
            this.Property(t => t.DAT_CADASTRO).HasColumnName("DAT_CADASTRO");
            this.Property(t => t.IND_RESP_PARC_PEND).HasColumnName("IND_RESP_PARC_PEND");
            this.Property(t => t.COD_IND_TIPO).HasColumnName("COD_IND_TIPO");
            this.Property(t => t.COD_IND_FORMA).HasColumnName("COD_IND_FORMA");
            this.Property(t => t.COD_IND_SATISFACAO).HasColumnName("COD_IND_SATISFACAO");
            this.Property(t => t.DSC_SATISFACAO).HasColumnName("DSC_SATISFACAO");
            this.Property(t => t.DSC_SATISFACAO_HTML).HasColumnName("DSC_SATISFACAO_HTML");
            this.Property(t => t.NUM_PROTOCOLO).HasColumnName("NUM_PROTOCOLO");
            this.Property(t => t.IND_FECHADO).HasColumnName("IND_FECHADO");
            this.Property(t => t.NUM_DIAS_ATRASO).HasColumnName("NUM_DIAS_ATRASO");
            this.Property(t => t.IND_VENCE_HOJE).HasColumnName("IND_VENCE_HOJE");
            this.Property(t => t.COD_IND_CANAL).HasColumnName("COD_IND_CANAL");
            this.Property(t => t.COD_ULT_RESPONSAVEL).HasColumnName("COD_ULT_RESPONSAVEL");
            this.Property(t => t.NOM_ULT_RESPONSAVEL).HasColumnName("NOM_ULT_RESPONSAVEL");
            this.Property(t => t.COD_ULT_DEPTO_RESP).HasColumnName("COD_ULT_DEPTO_RESP");
            this.Property(t => t.NOM_ULT_DEPTO_RESP).HasColumnName("NOM_ULT_DEPTO_RESP");
            this.Property(t => t.DSC_TEMPO_FILA).HasColumnName("DSC_TEMPO_FILA");
            this.Property(t => t.NUM_TEMPO_FILA).HasColumnName("NUM_TEMPO_FILA");
            this.Property(t => t.COD_OCORRENCIA_LEGADO).HasColumnName("COD_OCORRENCIA_LEGADO");
            this.Property(t => t.DSC_ABERTURA).HasColumnName("DSC_ABERTURA");
            this.Property(t => t.DSC_ABERTURA_HTML).HasColumnName("DSC_ABERTURA_HTML");

            this.Property(t => t.COD_ULT_SIT_TRAMITE).HasColumnName("COD_ULT_SIT_TRAMITE");
            this.Property(t => t.DSC_ULT_SIT_TRAMITE).HasColumnName("DSC_ULT_SIT_TRAMITE");
            this.Property(t => t.COD_DEPTO_ORIGEM).HasColumnName("COD_DEPTO_ORIGEM");
            this.Property(t => t.NOM_DEPTO_ORIGEM).HasColumnName("NOM_DEPTO_ORIGEM");
            this.Property(t => t.COD_USU_DEMANDANTE).HasColumnName("COD_USU_DEMANDANTE");
            this.Property(t => t.NOM_USU_DEMANDANTE).HasColumnName("NOM_USU_DEMANDANTE");
            this.Property(t => t.IND_EXIBE_RESPONSAVEL_SOLICIT).HasColumnName("IND_EXIBE_RESPONSAVEL_SOLICIT");
            this.Property(t => t.IND_ENVIO_MSG_WHATSAPP).HasColumnName("IND_ENVIO_MSG_WHATSAPP");

            this.HasMany(t => t.SOLICITACAO_ANEXO)
                .WithRequired(a => a.SOLICITACAO)
                .HasForeignKey(a => a.COD_SOLICITACAO)
                .WillCascadeOnDelete();

            this.HasMany(t => t.SOLICITACAO_HISTORICO_STATUS)
                .WithRequired(h => h.SOLICITACAO)
                .HasForeignKey(h => h.COD_SOLICITACAO);
        }
    }

    internal class SOLICITACAO_TRAMITEMap : EntityTypeConfiguration<SOLICITACAO_TRAMITE>
    {
        public SOLICITACAO_TRAMITEMap()
        {
            // Primary Key
            this.HasKey(t => t.COD_SOLICIT_TRAMITE);

            // Properties
            this.Property(t => t.COD_SOLICIT_TRAMITE)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.IND_RESOLVIDO)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.COD_IND_SITUACAO)
                .IsRequired();

            this.Property(t => t.COD_IND_TRAMITACAO)
                .IsRequired();

            this.Property(t => t.DSC_TRAMITE)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.DSC_TRAMITE_HTML)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.NOM_USU_SIGESP)
                .HasMaxLength(200);

            this.Property(t => t.NOM_USU_SGI)
                .HasMaxLength(200);

            this.Property(t => t.DSC_LOTACAO_SIGESP)
                .HasMaxLength(200);

            this.Property(t => t.DAT_CADASTRO).IsRequired();

            this.Property(t => t.DSC_DIAS_TRAMITE)
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("SOLICITACAO_TRAMITE");
            this.Property(t => t.COD_SOLICIT_TRAMITE).HasColumnName("COD_SOLICIT_TRAMITE");
            this.Property(t => t.COD_SOLICITACAO).HasColumnName("COD_SOLICITACAO");
            this.Property(t => t.IND_RESOLVIDO).HasColumnName("IND_RESOLVIDO");
            this.Property(t => t.COD_IND_SITUACAO).HasColumnName("COD_IND_SITUACAO");
            this.Property(t => t.COD_IND_TRAMITACAO).HasColumnName("COD_IND_TRAMITACAO");
            this.Property(t => t.DSC_TRAMITE).HasColumnName("DSC_TRAMITE");
            this.Property(t => t.COD_DOC_ANEXO).HasColumnName("COD_DOC_ANEXO");
            this.Property(t => t.COD_USU_SIGESP).HasColumnName("COD_USU_SIGESP");
            this.Property(t => t.NOM_USU_SIGESP).HasColumnName("NOM_USU_SIGESP");
            this.Property(t => t.COD_LOTACAO_SIGESP).HasColumnName("COD_LOTACAO_SIGESP");
            this.Property(t => t.DSC_LOTACAO_SIGESP).HasColumnName("DSC_LOTACAO_SIGESP");
            this.Property(t => t.COD_CIDADAO).HasColumnName("COD_CIDADAO");
            this.Property(t => t.COD_ENC_SOLICIT_TRAMITE).HasColumnName("COD_ENC_SOLICIT_TRAMITE");
            this.Property(t => t.DAT_REAGENDAMENTO).HasColumnName("DAT_REAGENDAMENTO");
            this.Property(t => t.DAT_CADASTRO).HasColumnName("DAT_CADASTRO");
            this.Property(t => t.COD_USU_SGI).HasColumnName("COD_USU_SGI");
            this.Property(t => t.NOM_USU_SGI).HasColumnName("NOM_USU_SGI");
            this.Property(t => t.DSC_TRAMITE_HTML).HasColumnName("DSC_TRAMITE_HTML");
            this.Property(t => t.COD_PESSOA_OUV).HasColumnName("COD_PESSOA_OUV");
            this.Property(t => t.COD_USU_INCLUSAO).HasColumnName("COD_USU_INCLUSAO");
            this.Property(t => t.DSC_CONCLUSAO_TECNICA).HasColumnName("DSC_CONCLUSAO_TECNICA");
            this.Property(t => t.COD_IND_CANAL).HasColumnName("COD_IND_CANAL");
            this.Property(t => t.COD_EQUIPE).HasColumnName("COD_EQUIPE");
            this.Property(t => t.DSC_DEPTO_NOTIFICADOS).HasColumnName("DSC_DEPTO_NOTIFICADOS");
            this.Property(t => t.DSC_DIAS_TRAMITE).HasColumnName("DSC_DIAS_TRAMITE");
            this.Property(t => t.IND_NOTIF_WP).HasColumnName("IND_NOTIF_WP");
            this.Property(t => t.COD_REQUEST_WPP).HasColumnName("COD_REQUEST_WPP");
            this.Property(t => t.COD_USU_ENCAMINHADO).HasColumnName("COD_USU_ENCAMINHADO");

            // Relationships

            this.HasRequired(t => t.SOLICITACAO)
                   .WithMany(s => s.SOLICITACAO_TRAMITE)
                   .HasForeignKey(t => t.COD_SOLICITACAO);

        }
    }

    internal class SOLICITACAO_ANEXOMap : EntityTypeConfiguration<SOLICITACAO_ANEXO>
    {
        public SOLICITACAO_ANEXOMap()
        {
            this.ToTable("SOLICITACAO_ANEXO");


            this.HasKey(t => t.COD_ANEXO);

            this.Property(t => t.COD_ANEXO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.NOM_ARQUIVO).HasMaxLength(255).IsRequired();
            this.Property(t => t.DSC_EXTENSAO).HasMaxLength(10).IsRequired();

            this.HasRequired(t => t.SOLICITACAO)
                   .WithMany(s => s.SOLICITACAO_ANEXO)
                   .HasForeignKey(t => t.COD_SOLICITACAO);
        }
    }

    internal class SOLICITACAO_HISTORICO_STATUSMap : EntityTypeConfiguration<SOLICITACAO_HISTORICO_STATUS>
    {
        public SOLICITACAO_HISTORICO_STATUSMap()
        {
            this.ToTable("SOLICITACAO_HISTORICO_STATUS");

            this.HasKey(t => t.COD_HISTORICO);
            this.Property(t => t.COD_HISTORICO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.DAT_ALTERACAO).IsRequired();

            this.HasRequired(t => t.SOLICITACAO)
                   .WithMany(s => s.SOLICITACAO_HISTORICO_STATUS)
                   .HasForeignKey(t => t.COD_SOLICITACAO);
        }
    }

    internal class SOLICITACAO_TRAMITE_COMENTARIOMap : EntityTypeConfiguration<SOLICITACAO_TRAMITE_COMENTARIO>
    {
        public SOLICITACAO_TRAMITE_COMENTARIOMap()
        {
            this.ToTable("SOLICITACAO_TRAMITE_COMENTARIO");

            this.HasKey(t => t.COD_COMENTARIO);
            this.Property(t => t.COD_COMENTARIO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.TXT_COMENTARIO).HasMaxLength(4000).IsRequired();

            this.HasRequired(t => t.SOLICITACAO_TRAMITE)
                   .WithMany(s => s.COMENTARIOS)
                   .HasForeignKey(t => t.COD_SOLICIT_TRAMITE);
        }
    }

    internal class SOLICITACAO_TRAMITE_LOGMap : EntityTypeConfiguration<SOLICITACAO_TRAMITE_LOG>
    {
        public SOLICITACAO_TRAMITE_LOGMap()
        {
            // Table & Column Mappings
            this.ToTable("SOLICITACAO_TRAMITE_LOG");

            // Primary Key
            this.HasKey(t => t.COD_LOG_TRAMITE);

            // Properties
            this.Property(t => t.COD_LOG_TRAMITE)
                .HasColumnName("COD_LOG_TRAMITE")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.COD_SOLICIT_TRAMITE)
                .IsRequired()
                .HasColumnName("COD_SOLICIT_TRAMITE");

            this.Property(t => t.DSC_LOG)
                .IsRequired()
                .HasMaxLength(4000)
                .HasColumnName("DSC_LOG");

            this.HasRequired(t => t.SOLICITACAO_TRAMITE)
                   .WithMany(s => s.LOGS)
                   .HasForeignKey(t => t.COD_SOLICIT_TRAMITE)
                   .WillCascadeOnDelete();
        }
    }
}
