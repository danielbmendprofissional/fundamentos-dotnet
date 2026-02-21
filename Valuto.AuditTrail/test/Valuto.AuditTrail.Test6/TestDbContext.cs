using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions;
using Valuto.AuditTrail.EFCore6;

namespace Valuto.AuditTrail.Test6
{
    public class TestDbContext : ValutoDbContext<TestDbContext>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) { }

        public DbSet<SOLICITACAO> Solicitacao { get; set; }
        public DbSet<SOLICITACAO_TRAMITE> SolicitacaoTramite { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SOLICITACAOMap());
            modelBuilder.ApplyConfiguration(new SOLICITACAO_TRAMITEMap());
            modelBuilder.ApplyConfiguration(new SOLICITACAO_ANEXOMap());
            modelBuilder.ApplyConfiguration(new SOLICITACAO_HISTORICO_STATUSMap());
            modelBuilder.ApplyConfiguration(new SOLICITACAO_TRAMITE_COMENTARIOMap());
            modelBuilder.ApplyConfiguration(new SOLICITACAO_TRAMITE_LOGMap());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnBeforeSaveChanges()
        {
            Console.WriteLine("passando pelo saveChanges do user");
        }

        protected override async Task OnBeforeSaveChangesAsync()
        {
            Console.WriteLine("passando pelo saveChanges do user");
        }

    }
    [ValutoAuditable]
    public class SOLICITACAO
    {
        public SOLICITACAO()
        {
            this.SOLICITACAO_TRAMITE = new List<SOLICITACAO_TRAMITE>();
            this.SOLICITACAO_ANEXO = new List<SOLICITACAO_ANEXO>();
            this.SOLICITACAO_HISTORICO_STATUS = new List<SOLICITACAO_HISTORICO_STATUS>();
        }

        public decimal COD_SOLICITACAO { get; set; }
        public decimal NUM_SOLICITACAO { get; set; }
        public string NUM_PROTO_EXTERNO { get; set; }
        public System.DateTime DAT_CADASTRO { get; set; }
        public string IND_RESP_PARC_PEND { get; set; }
        public decimal COD_IND_TIPO { get; set; }
        public decimal COD_IND_FORMA { get; set; }
        public decimal? COD_IND_SATISFACAO { get; set; }
        public string DSC_SATISFACAO { get; set; }
        public string DSC_SATISFACAO_HTML { get; set; }
        public string NUM_PROTOCOLO { get; set; }
        public string IND_FECHADO { get; set; }
        public decimal? NUM_DIAS_ATRASO { get; set; }
        public decimal? NUM_DIAS_VENCER { get; set; }
        public DateTime? DAT_VENCIMENTO { get; set; }
        public string IND_VENCE_HOJE { get; set; }
        public decimal COD_IND_CANAL { get; set; }
        public decimal? COD_ULT_RESPONSAVEL { get; set; }
        public string NOM_ULT_RESPONSAVEL { get; set; }
        public decimal? COD_ULT_DEPTO_RESP { get; set; }
        public string NOM_ULT_DEPTO_RESP { get; set; }
        public string DSC_TEMPO_FILA { get; set; }
        public double? NUM_TEMPO_FILA { get; set; }

        public decimal? COD_ULT_SIT_TRAMITE { get; set; }
        public string DSC_ULT_SIT_TRAMITE { get; set; }
        public decimal? COD_DEPTO_ORIGEM { get; set; }
        public string NOM_DEPTO_ORIGEM { get; set; }
        public decimal? COD_USU_DEMANDANTE { get; set; }
        public string NOM_USU_DEMANDANTE { get; set; }

        public string DSC_ABERTURA_HTML { get; set; }
        public string DSC_ABERTURA { get; set; }
        public string IND_ENVIO_MSG_WHATSAPP { get; set; }

        public decimal? COD_OCORRENCIA_LEGADO { get; set; }
        public string IND_EXIBE_RESPONSAVEL_SOLICIT { get; set; }
        public virtual ICollection<SOLICITACAO_TRAMITE> SOLICITACAO_TRAMITE { get; set; }
        public virtual ICollection<SOLICITACAO_ANEXO> SOLICITACAO_ANEXO { get; set; }
        public virtual ICollection<SOLICITACAO_HISTORICO_STATUS> SOLICITACAO_HISTORICO_STATUS { get; set; }
    }

    [ValutoAuditable]
    public partial class SOLICITACAO_TRAMITE
    {
        public SOLICITACAO_TRAMITE()
        {
            this.COMENTARIOS = new List<SOLICITACAO_TRAMITE_COMENTARIO>();
            this.LOGS = new List<SOLICITACAO_TRAMITE_LOG>();
        }

        public decimal COD_SOLICIT_TRAMITE { get; set; }
        public decimal COD_SOLICITACAO { get; set; }
        public string IND_RESOLVIDO { get; set; }
        public decimal COD_IND_SITUACAO { get; set; }
        public decimal COD_IND_TRAMITACAO { get; set; }
        public string DSC_TRAMITE { get; set; }
        public Nullable<decimal> COD_DOC_ANEXO { get; set; }
        public Nullable<decimal> COD_USU_SIGESP { get; set; }
        public string NOM_USU_SIGESP { get; set; }
        public Nullable<decimal> COD_LOTACAO_SIGESP { get; set; }
        public string DSC_LOTACAO_SIGESP { get; set; }
        public Nullable<decimal> COD_CIDADAO { get; set; }
        public Nullable<decimal> COD_ENC_SOLICIT_TRAMITE { get; set; }
        public Nullable<System.DateTime> DAT_REAGENDAMENTO { get; set; }
        public System.DateTime DAT_CADASTRO { get; set; }
        public Nullable<decimal> COD_USU_SGI { get; set; }
        public string NOM_USU_SGI { get; set; }
        public string DSC_TRAMITE_HTML { get; set; }
        public Nullable<decimal> COD_PESSOA_OUV { get; set; }
        public Nullable<decimal> COD_USU_INCLUSAO { get; set; }
        public Nullable<decimal> COD_IND_CANAL { get; set; }
        public Nullable<decimal> COD_EQUIPE { get; set; }
        public string DSC_CONCLUSAO_TECNICA { get; set; }
        public string DSC_DEPTO_NOTIFICADOS { get; set; }
        //public byte[] FOTO_USU_AD { get; set; }
        public Nullable<decimal> COD_USU_ENCAMINHADO { get; set; }
        public Nullable<decimal> COD_DEPTO_ENCAMINHADO { get; set; }
        public string NOM_DESTINATARIO { get; set; }
        public string DSC_DIAS_TRAMITE { get; set; }
        public string IND_NOTIF_WP { get; set; }
        public string COD_REQUEST_WPP { get; set; }

        public virtual SOLICITACAO SOLICITACAO { get; set; }

        public virtual ICollection<SOLICITACAO_TRAMITE_COMENTARIO> COMENTARIOS { get; set; }
        public virtual ICollection<SOLICITACAO_TRAMITE_LOG> LOGS { get; set; }
    }

    [ValutoAuditable]
    public class SOLICITACAO_ANEXO
    {
        public decimal COD_ANEXO { get; set; }
        public decimal COD_SOLICITACAO { get; set; }
        public string NOM_ARQUIVO { get; set; }
        public string DSC_EXTENSAO { get; set; }
        public virtual SOLICITACAO SOLICITACAO { get; set; }
    }

    [ValutoAuditable]
    public class SOLICITACAO_HISTORICO_STATUS
    {
        public decimal COD_HISTORICO { get; set; }
        public decimal COD_SOLICITACAO { get; set; }
        public DateTime DAT_ALTERACAO { get; set; }
        public decimal COD_IND_SITUACAO { get; set; }
        public virtual SOLICITACAO SOLICITACAO { get; set; }
    }

    [ValutoAuditable]
    public class SOLICITACAO_TRAMITE_COMENTARIO
    {
        public decimal COD_COMENTARIO { get; set; }
        public decimal COD_SOLICIT_TRAMITE { get; set; }
        public string TXT_COMENTARIO { get; set; }
        public virtual SOLICITACAO_TRAMITE SOLICITACAO_TRAMITE { get; set; }
    }

    [ValutoAuditable]
    public class SOLICITACAO_TRAMITE_LOG
    {
        public decimal COD_LOG_TRAMITE { get; set; }
        public decimal COD_SOLICIT_TRAMITE { get; set; }
        public string DSC_LOG { get; set; }
        public virtual SOLICITACAO_TRAMITE SOLICITACAO_TRAMITE { get; set; }
    }
}
